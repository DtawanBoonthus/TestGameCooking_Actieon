using System;
using System.Collections.Generic;
using Cooking.Services;
using Cooking.Utilities;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Cooking
{
    public class FoodRecipeView : MonoBehaviour
    {
        [SerializeField] private GenericButton leftButton = null!;
        [SerializeField] private GenericButton rightButton = null!;
        [SerializeField] private GameObject pagePrefab = null!;
        [SerializeField] private RectTransform pagePanel = null!;
        [SerializeField] private Sprite selectedPageSprite = null!;
        [SerializeField] private Sprite unselectedPageSprite = null!;
        [SerializeField] private List<FoodRecipe> foodRecipes = new();
        [SerializeField] private FilterFoodRecipe filterFoodRecipe = null!;
        [SerializeField] private TMP_InputField searchInputField = null!;

        [Inject] private readonly IFoodRecipeViewModel foodRecipeViewModel = null!;
        [Inject] private readonly IAddressableImageService imageService = null!;

        private CompositeDisposable? disposables;
        private readonly Dictionary<int, Image> pageImages = new();

        private void OnEnable()
        {
            searchInputField.text = string.Empty;
            searchInputField.onValueChanged.AddListener(OnSearchChanged);
            filterFoodRecipe.OnOptionClicked += FilterFoodRecipeOnOptionClicked;
            disposables = new();
            leftButton.SetClickCallback(foodRecipeViewModel.PreviousPage);
            rightButton.SetClickCallback(foodRecipeViewModel.NextPage);
            InitializeFoodRecipeView();
            BindRecipePageChanged();
            BindCurrentFoodIdsInPage();
            ResetFoodRecipeSelection();
        }

        private void OnDisable()
        {
            searchInputField.onValueChanged.RemoveListener(OnSearchChanged);
            filterFoodRecipe.OnOptionClicked -= FilterFoodRecipeOnOptionClicked;
            foodRecipeViewModel.UpdateCurrentFoodId(string.Empty);
            foreach (var page in pageImages.Values)
            {
                Destroy(page.gameObject);
            }

            foreach (var foodRecipe in foodRecipes)
            {
                foodRecipe.ClearClickCallback();
            }

            pageImages.Clear();
            leftButton.ClearClickCallback();
            rightButton.ClearClickCallback();
            disposables?.Dispose();
            Debug.Log($"{nameof(FoodRecipeView)}: Disposed");
        }

        private void OnSearchChanged(string text)
        {
            foodRecipeViewModel.ApplySearch(text);
        }

        private void FilterFoodRecipeOnOptionClicked(FilterFoodRecipeType filterFoodRecipeType)
        {
            foodRecipeViewModel.FilterFoodRecipeBy(filterFoodRecipeType);
        }

        private void InitializeFoodRecipeView()
        {
            foodRecipeViewModel.RefreshRecipePageCount();
        }

        private void UpdateFoodRecipeDisplay(IReadOnlyList<string> foodIds)
        {
            for (int i = 0; i < foodRecipes.Count; i++)
            {
                if (i < foodIds.Count)
                {
                    var foodId = foodIds[i];
                    var food = foodRecipeViewModel.GetFood(foodId);

                    var sprite = imageService.TryGetFromCache(food.ImageName);

                    if (sprite == null)
                    {
                        throw new Exception($"Can't find sprite for food id:'{foodId}', image name:'{food.ImageName}'");
                    }

                    foodRecipes[i].SetDisplayFoodInfo(food.Name, food.Rank, sprite);
                    foodRecipes[i].gameObject.SetActive(true);
                    foodRecipes[i].SetFoodId(foodId);
                    foodRecipes[i].SetClickCallback(() => OnFoodRecipeClicked(foodId));
                    continue;
                }

                foodRecipes[i].ClearClickCallback();
                foodRecipes[i].gameObject.SetActive(false);
            }
        }

        private void OnFoodRecipeClicked(string foodId)
        {
            foodRecipeViewModel.UpdateCurrentFoodId(foodId);
        }

        private void RebuildPageDot(int pageCount)
        {
            foreach (var page in pageImages.Values)
            {
                Destroy(page.gameObject);
            }

            pageImages.Clear();

            for (var i = 0; i < pageCount; i++)
            {
                var page = Instantiate(pagePrefab, pagePanel).GetComponent<Image>();
                pageImages.Add(i, page);
            }

            pageImages[foodRecipeViewModel.CurrentPageIndex.CurrentValue].sprite = selectedPageSprite;
        }

        private void BindRecipePageChanged()
        {
            if (disposables == null)
            {
                Debug.LogWarning($"{nameof(FoodRecipeView)}: Disposables is null");
                return;
            }

            foodRecipeViewModel.CurrentCountFoodIds.Subscribe(_ => RebuildPageDot(foodRecipeViewModel.RecipePageCount.CurrentValue)).AddTo(disposables);
            foodRecipeViewModel.CurrentPageIndex.Pairwise()
                .Subscribe(pair =>
                {
                    var (prev, curr) = pair;
                    if (prev >= 0 && pageImages.TryGetValue(prev, out var prevImg))
                    {
                        prevImg.sprite = unselectedPageSprite;
                    }

                    if (curr >= 0 && pageImages.TryGetValue(curr, out var currImg))
                    {
                        currImg.sprite = selectedPageSprite;
                    }
                }).AddTo(disposables);
        }

        private void BindCurrentFoodIdsInPage()
        {
            if (disposables == null)
            {
                Debug.LogWarning($"{nameof(FoodRecipeView)}: Disposables is null");
                return;
            }

            foodRecipeViewModel.CurrentFoodIdsInPage.Subscribe(UpdateFoodRecipeDisplay).AddTo(disposables);
        }

        private void ResetFoodRecipeSelection()
        {
            if (disposables == null)
            {
                Debug.LogWarning($"{nameof(FoodRecipeView)}: Disposables is null");
                return;
            }

            foodRecipeViewModel.CurrentFoodId.Subscribe(foodId =>
            {
                foreach (var foodRecipe in foodRecipes)
                {
                    foodRecipe.SetSelected(foodRecipe.FoodId == foodId);
                }
            }).AddTo(disposables);
        }
    }
}