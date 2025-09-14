using System.Collections.Generic;
using Cooking.Services;
using R3;
using UnityEngine;
using VContainer;

namespace Cooking
{
    public class IngredientView : MonoBehaviour
    {
        [SerializeField] private List<IngredientSlot> ingredientSlots = new();

        [Inject] private readonly IAddressableImageService imageService = null!;
        [Inject] private readonly IFoodRecipeViewModel foodRecipeViewModel = null!;

        private CompositeDisposable? disposables;

        private void OnEnable()
        {
            disposables = new CompositeDisposable();

            foodRecipeViewModel.CurrentFoodId.Subscribe(foodId =>
            {
                if (string.IsNullOrWhiteSpace(foodId))
                {
                    foreach (var slot in ingredientSlots)
                    {
                        slot.gameObject.SetActive(false);
                    }
                }

                var food = foodRecipeViewModel.GetFood(foodId);

                for (var i = 0; i < ingredientSlots.Count; i++)
                {
                    if (i < food.Ingredients.Count)
                    {
                        var ingredient = food.Ingredients[i];
                        var sprite = imageService.TryGetFromCache(foodRecipeViewModel.GetIngredient(ingredient.IngredientId).ImageName);

                        if (sprite == null)
                        {
                            Debug.LogError($"Can't find sprite for ingredient id:'{ingredient.IngredientId}'");
                            return;
                        }

                        ingredientSlots[i].SetIngredient(sprite, foodRecipeViewModel.PlayerData.Ingredients.CurrentValue[ingredient.IngredientId], ingredient.Amount);
                        ingredientSlots[i].gameObject.SetActive(true);
                        continue;
                    }

                    ingredientSlots[i].gameObject.SetActive(false);
                }
            }).AddTo(disposables);

            foodRecipeViewModel.PlayerData.Ingredients.Subscribe(ingredients =>
            {
                var foodId = foodRecipeViewModel.CurrentFoodId.CurrentValue;
                var food = foodRecipeViewModel.GetFood(foodId);

                for (var i = 0; i < ingredientSlots.Count; i++)
                {
                    if (i >= food.Ingredients.Count)
                    {
                        return;
                    }

                    var ingredient = food.Ingredients[i];
                    ingredientSlots[i].SetIngredientCount(ingredients[ingredient.IngredientId], ingredient.Amount);
                }
            }).AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables?.Dispose();
        }
    }
}