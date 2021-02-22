using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Recipe.Api.Models.Abstract;
using Recipe.Api.Models.Context;

namespace Recipe.Api.Models.DefaultContextModels
{
    [Table(nameof(Recipe))]
    public class Recipe: PrimaryKeyModel
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Instruction> Instructions { get; set; }
        
        
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Recipe>(e =>
            {
                e.HasMany(p => p.Ingredients);
                e.HasMany(p => p.Instructions);
            });
        }
        public void DeleteRemovedRelationships(DefaultContext context)
        {
            var ingredients = Ingredients.Select(i => i.Id).ToList();
            var ingredientsToDelete = context.Ingredients.Where(p => p.RecipeId == Id && !ingredients.Contains(p.Id)).ToList();
            context.RemoveRange(ingredientsToDelete);
            
            var instructions = Ingredients.Select(i => i.Id).ToList();
            var instructionsToDelete = context.Instructions.Where(p => p.RecipeId == Id && !instructions.Contains(p.Id)).ToList();
            context.RemoveRange(instructionsToDelete);
        }
    }
}