using Cooking.Utilities;
using UnityEngine;

namespace Cooking
{
    public class FilterFoodRecipeOptionButtons : GenericButton
    {
        [SerializeField] private FilterFoodRecipeType filterFoodRecipeType;
        [SerializeField] private Sprite selectedSprite = null!;
        [SerializeField] private Sprite unselectedSprite = null!;

        public FilterFoodRecipeType FilterFoodRecipeType => filterFoodRecipeType;
        public Sprite SelectedSprite => selectedSprite;
        public Sprite UnselectedSprite => unselectedSprite;
    }
}