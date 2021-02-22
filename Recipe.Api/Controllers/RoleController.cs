using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe.Api.Models.DefaultContextModels;

namespace Recipe.Api.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        public RoleController(BaseControllerDependencies context) : base(context) { }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await DatabaseContext.Roles.ToListAsync());
        }
        
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var role = await DatabaseContext.Roles.FindAsync(id);
        
            if (role == null)
            {
                return NotFound();
            }
        
            return Ok(role);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Role role)
        {
            var userRole = new Role
            {
                RoleName = role.RoleName
            };
            DatabaseContext.Roles.Add(userRole);
            await DatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), userRole);
        }
    }
}