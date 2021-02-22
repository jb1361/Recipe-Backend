using System.Collections.Generic;

namespace Recipe.Api.Models.Responses
{
    public class RecipeStateResponse
    {
        public Dictionary<int, DefaultContextModels.Recipe> Recipes { get; set; }
    }
}