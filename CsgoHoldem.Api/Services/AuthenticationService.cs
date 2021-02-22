using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CsgoHoldem.Api.Config;
using CsgoHoldem.Api.Models.Context;
using CsgoHoldem.Api.Models.DefaultContextModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CsgoHoldem.Api.Services
{
    public interface IAuthenticationService
    {
        Task<User> Authenticate(string email, string password);
        User MockAuthenticate();
        Task<User> AddUser(User user);
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppSettings _appSettings;
        private readonly DefaultContext _databaseContext;

        public AuthenticationService(AppSettings appSettings, DefaultContext defaultContext)
        {
            _appSettings = appSettings;
            _databaseContext = defaultContext;
        }
        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _databaseContext.Users
                .Where(u => u.ArchivedAt == null)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(x => x.Email == email);
            // return null if user not found
            if (user == null)
                return null;
            
            var hasher = new PasswordHasher<User>();
            if (hasher.VerifyHashedPassword(user, user.Password, password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            
            // remove password before returning
            user.Password = null;
            user.Token = CreateToken(user);
            return user;
        }
        
        public User MockAuthenticate()
        {
            var user = new User
            {
                Email = _appSettings.MockUser.Email,
                Role =  new Role {Id = 0, RoleName = _appSettings.MockUser.Role},
                Id = 0,
            };
            user.Token = CreateToken(user);
            return user;
        }
        
        private string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JWTSecret);
            var claimsIdentity = new ClaimsIdentity();
            
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Id.ToString()));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, user.Role.RoleName));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User> AddUser(User user)
        {
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, user.Password);
            _databaseContext.Users.Add(user);
            await _databaseContext.SaveChangesAsync();
            return user;
        }
    }
}