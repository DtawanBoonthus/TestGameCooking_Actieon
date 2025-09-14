using System.Collections.Generic;

namespace Cooking;

public interface IGameConfigDatabase
{
    int RecipeCountPerPage { get; }
    int SearchCutoffScore { get; }
    IReadOnlyList<RequestIngredient> PlayerIngredients { get; }
    int PlayerEnergy { get; }
    int MaxCookingEnergy { get; }
    int EnergyRecoveryCount { get; }
    double EnergyRecoverySeconds { get; }
    int CookingEnergyCost { get; }
}