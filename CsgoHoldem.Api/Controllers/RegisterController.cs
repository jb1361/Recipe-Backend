using System.Linq;
using System.Threading.Tasks;
using CsgoHoldem.Api.Models.DefaultContextModels;
using CsgoHoldem.Api.Models.Requests;
using CsgoHoldem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CsgoHoldem.Api.Controllers
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