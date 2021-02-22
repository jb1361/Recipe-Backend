using System.Threading.Tasks;
using CsgoHoldem.Api.Models.Requests;
using CsgoHoldem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CsgoHoldem.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController: BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService, BaseControllerDependencies deps): base(deps)
        {
            _authenticationService = authenticationService;
        }
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateRequest userParam)
        {
            if(AppSettings.EnableDeveloperMode)
                return Ok(_authenticationService.MockAuthenticate());
            
            var user = await _authenticationService.Authenticate(userParam.Email, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
    }
}