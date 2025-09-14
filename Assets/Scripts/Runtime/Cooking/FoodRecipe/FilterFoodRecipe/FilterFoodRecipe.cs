using System;
using System.Collections.Generic;
using Cooking.Utilities;
using UnityEngine;

namespace Cooking
{
    public class FilterFoodRecipe : MonoBehaviour
    {
        [SerializeField] private GenericButton filterButton = null!;
        [SerializeField] private List<FilterFoodRecipeOptionButtons> filterFoodRecipeOptionButtons = new();
        [SerializeField] private RectTransform filterOptionPanel = null!;
        [SerializeField] private FilterClickCatcher filterClickCatcher = null!;

        public event Action<FilterFoodRecipeType>? OnOptionClicked;
        private FilterFoodRecipeType lastFilterFoodRecipeType;

        private void OnEnable()
        {
            lastFilterFoodRecipeType = FilterFoodRecipeType.All;

            foreach (var filterFoodRecipeOptionButton in filterFoodRecipeOptionButtons)
            {
                filterFoodRecipeOptionButton.SetClickCallback(() => { OnOptionButtonClick(filterFoodRecipeOptionButton); });
            }

            filterClickCatcher.SetOnCloseOptionPanel(OnFilterButtonClick);
            filterButton.SetClickCallback(OnFilterButtonClick);
        }

        private void OnOptionButtonClick(FilterFoodRecipeOptionButtons selectedOption)
        {
            if (lastFilterFoodRecipeType == selectedOption.FilterFoodRecipeType)
            {
                selectedOption.SetImage(selectedOption.UnselectedSprite);
                lastFilterFoodRecipeType = FilterFoodRecipeType.All;
                OnOptionClicked?.Invoke(lastFilterFoodRecipeType);
                return;
            }

            lastFilterFoodRecipeType = selectedOption.FilterFoodRecipeType;

            foreach (var filterFoodRecipeOptionButton in filterFoodRecipeOptionButtons)
            {
                if (selectedOption.GetHashCode().Equals(filterFoodRecipeOptionButton.GetHashCode()))
                {
                    filterFoodRecipeOptionButton.SetImage(filterFoodRecipeOptionButton.SelectedSprite);
                    continue;
                }

                filterFoodRecipeOptionButton.SetImage(filterFoodRecipeOptionButton.UnselectedSprite);
            }

            OnOptionClicked?.Invoke(selectedOption.FilterFoodRecipeType);
        }

        private void OnDisable()
        {
            filterClickCatcher.ClearOnCloseOptionPanel();
            filterButton.ClearClickCallback();
            filterOptionPanel.gameObject.SetActive(false);
            foreach (var filterFoodRecipeOptionButton in filterFoodRecipeOptionButtons)
            {
                filterFoodRecipeOptionButton.ClearClickCallback();
            }
        }

        private void OnFilterButtonClick()
        {
            filterClickCatcher.gameObject.SetActive(!filterClickCatcher.gameObject.activeSelf);
            filterOptionPanel.gameObject.SetActive(!filterOptionPanel.gameObject.activeSelf);
        }
    }
}