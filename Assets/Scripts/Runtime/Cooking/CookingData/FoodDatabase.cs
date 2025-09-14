using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking
{
    [CreateAssetMenu(fileName = "FoodDatabase", menuName = "Database/Food", order = 0)]
    public class FoodDatabase : ScriptableObject, IFoodDatabase
    {
        [SerializeField] private List<Food> foods = new();

        private Dictionary<string, Food> foodMapId = new();

        private void OnEnable()
        {
            foodMapId = foods.ToDictionary(x => x.Id, x => x);
        }

        public IReadOnlyDictionary<string, Food> Foods => foodMapId;

        public Food GetById(string foodId)
        {
            return foodMapId.TryGetValue(foodId, out var food)
                ? food
                : throw new KeyNotFoundException($"Food with id {foodId} not found.");
        }
    }
}