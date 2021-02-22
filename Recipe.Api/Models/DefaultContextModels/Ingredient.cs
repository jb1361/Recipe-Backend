using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Recipe.Api.Models.Abstract;

namespace Recipe.Api.Models.DefaultContextModels
{
    [Table(nameof(Ingredient))]
    public class Ingredient: PrimaryKeyModel
    {
        public string Measurement { get; set; }
        public string Name { get; set; }
        public int? RecipeId { get; set; }
        [ForeignKey(nameof(RecipeId))]
        [JsonIgnore]
        public Recipe Recipe { get; set; }
    }
}