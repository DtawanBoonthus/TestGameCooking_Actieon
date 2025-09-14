using System;
using System.Collections.Generic;
using System.Linq;
using Cooking.Services;
using Newtonsoft.Json;
using R3;
using VContainer;

namespace Cooking
{
    [Serializable]
    public class MockPlayerData : IPlayerData, IDisposable
    {
        [JsonProperty("ingredients")] private readonly ReactiveProperty<Dictionary<string, int>> ingredients = new();
        [JsonProperty("energy")] private readonly ReactiveProperty<int> energy = new(0);

        [Inject] private readonly IGameConfigDatabase gameConfigDatabase = null!;
        [Inject] private readonly ISaveLoadService saveLoadService = null!;

        public void InitializeData(Dictionary<string, int> initialIngredients, int initialEnergy)
        {
            ingredients.Value = initialIngredients;
            energy.Value = initialEnergy;
        }

        public void InitializeData(MockPlayerData mockPlayerData)
        {
            ingredients.Value = mockPlayerData.ingredients.CurrentValue;
            energy.Value = mockPlayerData.energy.CurrentValue;
        }

        [JsonIgnore] public ReadOnlyReactiveProperty<Dictionary<string, int>> Ingredients => ingredients;
        [JsonIgnore] public ReadOnlyReactiveProperty<int> Energy => energy;

        public void DecreaseIngredient(IReadOnlyDictionary<string, int> decreaseIngredients)
        {
            var results = new Dictionary<string, int>();

            foreach (var (ingredientId, amount) in decreaseIngredients)
            {
                if (ingredients.CurrentValue.TryGetValue(ingredientId, out var count) && count >= amount)
                {
                    results[ingredientId] = ingredients.CurrentValue[ingredientId] - amount;
                }
                else
                {
                    throw new Exception($"Ingredient {ingredientId} not found or not enough.");
                }
            }

            foreach (var (ingredientId, amount) in results)
            {
                ingredients.Value[ingredientId] = amount;
            }

            ingredients.ForceNotify();
        }

        public void DecreaseEnergy(int amount)
        {
            if (energy.Value < amount)
            {
                return;
            }

            energy.Value -= amount;
        }

        public void IncreaseEnergy(int amount)
        {
            energy.Value = Math.Clamp(amount + energy.CurrentValue, 0, gameConfigDatabase.MaxCookingEnergy);
        }

        public void ResetIngredient()
        {
            ingredients.Value = gameConfigDatabase.PlayerIngredients.ToDictionary(x => x.IngredientId, x => x.Amount);
        }

        public void Dispose()
        {
            saveLoadService.Save(SaveLoadKey.PLAYER_DATA, this);
            saveLoadService.Save(SaveLoadKey.LAST_ACTIVE_TIME, DateTime.UtcNow);
            ingredients.Dispose();
            energy.Dispose();
        }
    }
}