using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Recipe.Api.Models.Abstract;

namespace Recipe.Api.Models.DefaultContextModels
{
    [Table(nameof(Instruction))]
    public class Instruction: PrimaryKeyModel
    {
        public string Text { get; set; }
        public int? RecipeId { get; set; }
        [ForeignKey(nameof(RecipeId))]
        [JsonIgnore]
        public Recipe Recipe { get; set; }
    }
}