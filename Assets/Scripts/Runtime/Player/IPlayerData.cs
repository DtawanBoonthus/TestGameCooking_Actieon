using System.Collections.Generic;
using R3;

namespace Cooking;

public interface IPlayerData
{
    void InitializeData(Dictionary<string, int> initialIngredients, int initialEnergy);
    void InitializeData(MockPlayerData playerData);
    ReadOnlyReactiveProperty<Dictionary<string, int>> Ingredients { get; }
    ReadOnlyReactiveProperty<int> Energy { get; }
    void DecreaseIngredient(IReadOnlyDictionary<string, int> decreaseIngredients);
    void DecreaseEnergy(int amount);
    void IncreaseEnergy(int amount);
    void ResetIngredient();
}