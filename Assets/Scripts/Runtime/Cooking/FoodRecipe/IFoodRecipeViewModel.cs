using System.Collections.Generic;
using R3;

namespace Cooking;

public interface IFoodRecipeViewModel
{
    ReadOnlyReactiveProperty<int> RecipePageCount { get; }
    ReadOnlyReactiveProperty<int> CurrentPageIndex { get; }
    ReadOnlyReactiveProperty<IReadOnlyList<string>> CurrentFoodIdsInPage { get; }
    ReadOnlyReactiveProperty<int> CurrentCountFoodIds { get; }
    ReadOnlyReactiveProperty<string> CurrentFoodId { get; }

    /// <summary>
    /// Recalculates and updates the total number of recipe pages 
    /// based on the current food count and the configured recipes per page.
    ///
    /// This method should be invoked whenever related data changes, for example:
    /// - When foods are added, removed, or modified in <see cref="IFoodDatabase"/>
    /// - When the recipes-per-page setting changes in <see cref="IGameConfigDatabase"/>
    ///
    /// Can be triggered by events, observables, or called manually by
    /// the system that updates the underlying data.
    /// </summary>
    void RefreshRecipePageCount();

    void ApplySearch(string? query);
    void NextPage();
    void PreviousPage();
    void FilterFoodRecipeBy(FilterFoodRecipeType filterFoodRecipeType);
    Food GetFood(string foodId);
    Ingredient GetIngredient(string ingredientId);
    IPlayerData PlayerData { get; }
    void UpdateCurrentFoodId(string foodId);
}