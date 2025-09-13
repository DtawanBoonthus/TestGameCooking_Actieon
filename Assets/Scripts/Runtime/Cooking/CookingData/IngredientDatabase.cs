using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking.CookingData
{
    [CreateAssetMenu(fileName = "IngredientDatabase", menuName = "Database/Ingredient", order = 1)]
    public class IngredientDatabase : ScriptableObject, IIngredientDatabase
    {
        [SerializeField] private List<Ingredient> ingredients = new();

        private Dictionary<string, Ingredient> ingredientMapId = new();

        private void OnEnable()
        {
            ingredientMapId = ingredients.ToDictionary(x => x.Id, x => x);
        }

        public IReadOnlyDictionary<string, Ingredient> Ingredients => ingredientMapId;

        public Ingredient GetById(string ingredientId)
        {
            return ingredientMapId.TryGetValue(ingredientId, out var ingredient)
                ? ingredient
                : throw new KeyNotFoundException($"Ingredient with id {ingredientId} not found.");
        }
    }
}