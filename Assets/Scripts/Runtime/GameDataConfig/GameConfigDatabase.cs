using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking
{
    [CreateAssetMenu(fileName = "GameConfigDatabase", menuName = "Database/GameDataConfig", order = 2)]
    public class GameConfigDatabase : ScriptableObject, IGameConfigDatabase
    {
        [SerializeField] private int recipeCountPerPage;
        [SerializeField] private int searchCutoffScore;
        [SerializeField] private List<RequestIngredient> playerIngredients = new();
        [SerializeField] private int playerEnergy;
        [SerializeField] private int maxCookingEnergy;
        [SerializeField] private int energyRecoveryCount;
        [SerializeField] private double energyRecoverySeconds;
        [SerializeField] private int cookingEnergyCost = 10;

        public int RecipeCountPerPage => recipeCountPerPage;
        public int SearchCutoffScore => searchCutoffScore;
        public IReadOnlyList<RequestIngredient> PlayerIngredients => playerIngredients;
        public int PlayerEnergy => playerEnergy;
        public int MaxCookingEnergy => maxCookingEnergy;
        public int EnergyRecoveryCount => energyRecoveryCount;
        public double EnergyRecoverySeconds => energyRecoverySeconds;
        public int CookingEnergyCost => cookingEnergyCost;
    }
}