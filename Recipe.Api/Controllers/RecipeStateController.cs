using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe.Api.Models.Responses;

namespace Recipe.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RecipeStateController: BaseController
    {
        public RecipeStateController(BaseControllerDependencies dependencies) : base(dependencies)
        {
        }
        
        [HttpGet]
        public async Task<IActionResult> GetConfiguration()
        {
            return Ok(new RecipeStateResponse
            {
                Recipes = await DatabaseContext.Recipes.Include(r => r.Ingredients).Include(r => r.Instructions).ToDictionaryAsync(d => d.Id),
            });
        }
    }
}