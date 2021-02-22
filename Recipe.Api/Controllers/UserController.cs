using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe.Api.Models.DefaultContextModels;
using Recipe.Api.Models.Requests;
using Recipe.Api.Services;

namespace Recipe.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController: BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailService _emailService;

        public UserController(
            IAuthenticationService authenticationService,
            IEmailService emailService,
            BaseControllerDependencies deps): base(deps)
        {
            _authenticationService = authenticationService;
            _emailService = emailService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await DatabaseContext.Users.Include(u => u.Role).ToListAsync();
            foreach (var user in users)
            {
                user.SetNulls();
            }
            var userDict = users.ToDictionary(u => u.Id);
            return Ok(userDict);
        }
        
        [HttpPost("{id}/archive")]
        public async Task<IActionResult> ArchiveUser(int id)
        {
            var user = await DatabaseContext.Users.Where(u => u.ArchivedAt == null && u.Id == id).Include(u => u.Role).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            user.ArchivedAt = DateTime.Now;
            await DatabaseContext.SaveChangesAsync();
            return Ok(user);
        }
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var user = await DatabaseContext.Users.Where(u => u.ArchivedAt != null && u.Id == id).Include(u => u.Role).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            user.ArchivedAt = null;
            await DatabaseContext.SaveChangesAsync();
            return Ok(user);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await DatabaseContext.Users.Where(u => u.ArchivedAt == null && u.Id == id).FirstAsync();
        
            if (user == null)
            {
                return NotFound();
            }
            user.SetNulls();
            return Ok(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegistrationRequest userData)
        {
            if (!userData.IsValid(DatabaseContext))
                return BadRequest(userData.Validate(DatabaseContext));
            await DatabaseContext.Database.BeginTransactionAsync();
            var user = Map<User>(userData);
            user.Password = Guid.NewGuid().ToString();
            await _authenticationService.AddUser(user);
            user.SetNulls();
            await user.SetRole(DatabaseContext);
            await ResetUserPassword(user.Email);
            DatabaseContext.Database.CommitTransaction();
            return Ok(user);
        }
        
        [HttpGet("current")]
        public async Task<IActionResult> Current()
        {
            return Ok(await GetUser());
        }
        
        [HttpPost("forgotPassword/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // TODO Currently this route is susceptible to a timing attack.
            // The only information that could be gained from such attack would be valid account emails. 
            // The fix for this is to Ensure that the route always takes a set amount of time before response but this
            // is an extremely minor information exploit.
            if (!UserExists(email))
            {
                return Ok();
            }
            var user = await DatabaseContext.Users.Where(u => u.Email == email).FirstAsync();
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.ResetTokenValidDate = DateTime.Now.AddMinutes(60);
            await DatabaseContext.SaveChangesAsync();
            _emailService.SendTextEmail(email, "Forgot Password Request", 
                "Click the link below to reset your password. The link is valid for 20 minutes. " +
                "http://localhost:3000/reset-password?token=" + token + "&email=" + user.Email);
            return Ok();
        }
        
        [HttpGet("validateToken/{token}")]
        [AllowAnonymous]
        public IActionResult ValidatePasswordResetToken(string token)
        {
            if (IsPasswordResetTokenValid(token))
            {
                return Ok();
            }
            return BadRequest("Token is Invalid.");
        }
        
        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetRequest)
        {
            if (!IsPasswordResetTokenValid(resetRequest.PasswordResetToken))
            {
                return BadRequest("Token is Invalid.");
            }
            
            if (resetRequest.Password != resetRequest.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }
            
            await DatabaseContext.Database.BeginTransactionAsync();
            var validUser = DatabaseContext.Users.Any(u =>
                    u.Email == resetRequest.Email && u.PasswordResetToken == resetRequest.PasswordResetToken);
            if (!validUser)
            {
                return BadRequest("Invalid User.");
            }
            var user = await DatabaseContext.Users.Where(u =>
                u.Email == resetRequest.Email && u.PasswordResetToken == resetRequest.PasswordResetToken).FirstAsync();
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, resetRequest.ConfirmPassword);
            user.PasswordResetToken = null;
            user.ResetTokenValidDate = null;
            await DatabaseContext.SaveChangesAsync();
            DatabaseContext.Database.CommitTransaction();
            _emailService.SendTextEmail(user.Email, "Password Reset", "Your password has been reset.");
            return Ok();
        }
        
        public async Task<IActionResult> ResetUserPassword(string email)
        {
            if (!UserExists(email))
            {
                return NotFound();
            }
            var user = await DatabaseContext.Users.Where(u => u.Email == email).FirstAsync();
            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.ResetTokenValidDate = DateTime.Now.AddMonths(1);
            await DatabaseContext.SaveChangesAsync();
            _emailService.SendTextEmail(email, "Account Created", 
                "Click the link below to set you password." +
                "http://localhost:3000/reset-password?token=" + token + "&email=" + user.Email);
            return Ok();
        }
        
        private bool IsPasswordResetTokenValid(string token)
        {
            return DatabaseContext.Users.Any(
                u => u.PasswordResetToken != null &&
                     u.PasswordResetToken == token &&
                     DateTime.Now < u.ResetTokenValidDate);
        }
        
        private bool UserExists(string email)
        {
            return DatabaseContext.Users.Any(u => u.Email == email);
        }
    }
}