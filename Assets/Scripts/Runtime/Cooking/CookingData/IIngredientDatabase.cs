using System.Collections.Generic;

namespace Cooking
{
    public interface IIngredientDatabase
    {
        IReadOnlyDictionary<string, Ingredient> Ingredients { get; }
        Ingredient GetById(string ingredientId);
    }
}