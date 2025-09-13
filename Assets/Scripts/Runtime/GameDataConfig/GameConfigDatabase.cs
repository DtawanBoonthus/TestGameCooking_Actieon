using UnityEngine;

namespace Cooking
{
    [CreateAssetMenu(fileName = "GameConfigDatabase", menuName = "Database/GameDataConfig", order = 2)]
    public class GameConfigDatabase : ScriptableObject, IGameConfigDatabase
    {
        [SerializeField] private int recipeCountPerPage;
        [SerializeField] private int searchCutoffScore;

        public int RecipeCountPerPage => recipeCountPerPage;
        public int SearchCutoffScore => searchCutoffScore;
    }
}