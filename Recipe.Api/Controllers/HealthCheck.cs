using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Recipe.Api.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HealthCheck : Controller {
        [HttpGet]
        public string GetHealthCheck() {
            return "All Good";
        }
    }
}