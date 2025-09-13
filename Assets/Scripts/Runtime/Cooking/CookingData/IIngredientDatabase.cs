using System.Collections.Generic;

namespace Cooking.CookingData
{
    public interface IIngredientDatabase
    {
        IReadOnlyDictionary<string, Ingredient> Ingredients { get; }
        Ingredient GetById(string ingredientId);
    }
}