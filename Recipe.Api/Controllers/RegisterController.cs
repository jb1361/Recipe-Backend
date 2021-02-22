using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipe.Api.Models.DefaultContextModels;
using Recipe.Api.Models.Requests;
using Recipe.Api.Services;


namespace Recipe.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegisterController: BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public RegisterController(IAuthenticationService authenticationService, BaseControllerDependencies deps): base(deps)
        {
            _authenticationService = authenticationService;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest registration)
        {
            if (!registration.IsValid(DatabaseContext))
                return BadRequest(registration.Validate(DatabaseContext));
            
            var user = Map<User>(registration);
            user.RoleId = DatabaseContext.Roles.First(x => x.RoleName == "User").Id;
            await _authenticationService.AddUser(user);
            return Ok(await _authenticationService.Authenticate(registration.GetEmail(), registration.Password));
        }
    }
}