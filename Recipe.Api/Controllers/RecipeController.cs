using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Recipe.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RecipeController : BaseController
    {
        public RecipeController(BaseControllerDependencies dependencies) : base(dependencies)
        {
        }
        // GET: api/v1/Recipe
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.DefaultContextModels.Recipe>>> GetRecipe()
        {
            return await DatabaseContext.Recipes.ToListAsync();
        }
        
        // GET: api/v1/Recipe/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.DefaultContextModels.Recipe>> GetRecipe(int id)
        {
            var recipe = await DatabaseContext.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }
        
        // POST: api/v1/Recipe
        [HttpPost]
        public async Task<ActionResult<Models.DefaultContextModels.Recipe>> UpsertRecipe(Models.DefaultContextModels.Recipe recipe)
        {
            await DatabaseContext.Database.BeginTransactionAsync();
            if (recipe.Id == 0) {
                DatabaseContext.Add(recipe); 
            } else
            {
                recipe.DeleteRemovedRelationships(DatabaseContext);
                DatabaseContext.Recipes.Update(recipe);
            }
            await DatabaseContext.SaveChangesAsync();
            DatabaseContext.Database.CommitTransaction();
            return Ok(recipe);
        }

        // DELETE: api/v1/Recipe/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Models.DefaultContextModels.Recipe>> DeleteRecipe(int id)
        {
            var recipe = await DatabaseContext.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            DatabaseContext.Recipes.Remove(recipe);
            await DatabaseContext.SaveChangesAsync();

            return recipe;
        }

        private bool RecipeExists(int id)
        {
            return DatabaseContext.Recipes.Any(e => e.Id == id);
        }
    }
}
