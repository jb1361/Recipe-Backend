using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Recipe.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserManagementController: BaseController
    {
        public UserManagementController(BaseControllerDependencies context) : base(context)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetConfiguration()
        {
            return Ok();
        }
    }
}