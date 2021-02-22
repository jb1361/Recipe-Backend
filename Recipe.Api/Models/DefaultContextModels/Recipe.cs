using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Recipe.Api.Models.Abstract;

namespace Recipe.Api.Models.DefaultContextModels
{
    [Table(nameof(Recipe))]
    public class Recipe: PrimaryKeyModel
    {
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
    }
}