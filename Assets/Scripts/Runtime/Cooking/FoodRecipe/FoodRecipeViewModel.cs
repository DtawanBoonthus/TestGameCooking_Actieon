using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FuzzySharp;
using R3;
using VContainer;

namespace Cooking;

public class FoodRecipeViewModel : IFoodRecipeViewModel
{
    private readonly ReactiveProperty<int> recipePageCount = new(-1);
    private readonly ReactiveProperty<int> currentPageIndex = new(-1);
    private readonly ReactiveProperty<int> currentCountFoodIds = new(-1);
    private readonly ReactiveProperty<IReadOnlyList<string>> currentFoodIdsInPage = new(new List<string>());
    public ReadOnlyReactiveProperty<int> RecipePageCount => recipePageCount;
    public ReadOnlyReactiveProperty<int> CurrentPageIndex => currentPageIndex;
    public ReadOnlyReactiveProperty<IReadOnlyList<string>> CurrentFoodIdsInPage => currentFoodIdsInPage;
    public ReadOnlyReactiveProperty<int> CurrentCountFoodIds => currentCountFoodIds;

    [Inject] private readonly IFoodDatabase foodDatabase = null!;
    [Inject] private readonly IGameConfigDatabase gameConfigDatabase = null!;

    private List<string> currentFoodIds = new();
    private FilterFoodRecipeType currentFilterFoodRecipeType = FilterFoodRecipeType.All;
    private List<string> defaultFoodSortedIds = new();
    private readonly Dictionary<int, List<string>> rankFoodSortedIds = new();
    private static bool IsThai(char c) => c >= 0x0E00 && c <= 0x0E7F;

    public void NextPage()
    {
        var next = currentPageIndex.CurrentValue + 1;
        currentPageIndex.Value = next == recipePageCount.CurrentValue ? 0 : next;
        currentFoodIdsInPage.Value = GetFoodIds(currentFoodIds, currentPageIndex.CurrentValue, gameConfigDatabase.RecipeCountPerPage).ToArray();
    }

    public void PreviousPage()
    {
        var prev = currentPageIndex.CurrentValue - 1;
        currentPageIndex.Value = prev < 0 ? recipePageCount.CurrentValue - 1 : prev;
        currentFoodIdsInPage.Value = GetFoodIds(currentFoodIds, currentPageIndex.CurrentValue, gameConfigDatabase.RecipeCountPerPage).ToArray();
    }

    public void FilterFoodRecipeBy(FilterFoodRecipeType filterFoodRecipeType)
    {
        currentFilterFoodRecipeType = filterFoodRecipeType;
        currentFoodIds = GetCurrentFoodIds();
        recipePageCount.Value = RecipeCountPerPage(currentFoodIds.Count, gameConfigDatabase.RecipeCountPerPage);
        currentPageIndex.Value = 0;
        UpdateCurrentFoodIds();
    }

    public Food GetFood(string foodId)
    {
        return foodDatabase.GetById(foodId);
    }

    /// <inheritdoc/>
    public void RefreshRecipePageCount()
    {
        SortFoods();
        currentFilterFoodRecipeType = FilterFoodRecipeType.All;
        currentFoodIds = defaultFoodSortedIds;
        recipePageCount.Value = RecipeCountPerPage(currentFoodIds.Count, gameConfigDatabase.RecipeCountPerPage);
        currentPageIndex.Value = 0;
        UpdateCurrentFoodIds();
    }

    public void ApplySearch(string? query)
    {
        var foodIds = GetCurrentFoodIds();
        if (string.IsNullOrWhiteSpace(query))
        {
            currentFoodIds = foodIds;
            recipePageCount.Value = RecipeCountPerPage(currentFoodIds.Count, gameConfigDatabase.RecipeCountPerPage);
            currentPageIndex.Value = 0;
            UpdateCurrentFoodIds();
            return;
        }

        var result = ProcessFoodSearchResults(foodIds, query);

        recipePageCount.Value = RecipeCountPerPage(result.Count, gameConfigDatabase.RecipeCountPerPage);
        currentPageIndex.Value = 0;
        currentFoodIds = result;
        UpdateCurrentFoodIds();
    }

    private List<string> ProcessFoodSearchResults(List<string> foodIds, string query)
    {
        var queryNorm = Norm(query);

        var items = foodIds
            .Select(id =>
            {
                var f = foodDatabase.GetById(id);
                var nameNorm = Norm(f.Name);
                var exact = nameNorm == queryNorm;
                var prefix = !exact && nameNorm.StartsWith(queryNorm);
                var contains = !exact && !prefix && nameNorm.Contains(queryNorm);
                var score = Fuzz.Ratio(nameNorm, queryNorm);
                return new
                {
                    id,
                    name = f.Name,
                    nameNorm,
                    exact,
                    prefix,
                    contains,
                    score
                };
            })
            .ToList();

        var exactHit = items.FirstOrDefault(x => x.exact);
        if (exactHit != null)
        {
            foodIds = new List<string> { exactHit.id };
        }
        else
        {
            var prefixHits = items
                .Where(x => x.prefix)
                .OrderBy(x => x.name.Length)
                .ThenBy(x => x.name, StringComparer.InvariantCultureIgnoreCase)
                .Select(x => x.id)
                .ToList();

            if (prefixHits.Count > 0)
            {
                foodIds = prefixHits;
            }
            else
            {
                var cutoff = gameConfigDatabase.SearchCutoffScore;
                var ranked = items
                    .Where(x => x.contains || x.score >= cutoff)
                    .OrderByDescending(x => x.contains)
                    .ThenByDescending(x => x.score)
                    .ThenBy(x => x.name, StringComparer.InvariantCultureIgnoreCase)
                    .Select(x => x.id)
                    .ToList();

                foodIds = ranked;
            }
        }

        return foodIds;

        static string Norm(string s) => s.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormC);
    }

    private static int RecipeCountPerPage(int foodCount, int recipeCountPerPage)
    {
        return (foodCount + recipeCountPerPage - 1) / recipeCountPerPage;
    }

    private void SortFoods()
    {
        rankFoodSortedIds.Clear();

        defaultFoodSortedIds = foodDatabase.Foods.Values.OrderBy(f => f.Rank)
            .ThenBy(f => !IsThai(f.Name[0]))
            .ThenBy(f => f.Name, StringComparer.Create(new CultureInfo("th-TH"), true))
            .Select(f => f.Id)
            .ToList();

        var groupByRank = foodDatabase.Foods.Values.GroupBy(f => f.Rank);

        foreach (var group in groupByRank)
        {
            rankFoodSortedIds.Add(
                group.Key,
                group.OrderBy(f => !IsThai(f.Name[0]))
                    .ThenBy(f => f.Name, StringComparer.Create(new CultureInfo("th-TH"), true))
                    .Select(f => f.Id)
                    .ToList()
            );
        }
    }

    private static IEnumerable<string> GetFoodIds(IEnumerable<string> foodIds, int pageIndex, int pageSize)
    {
        return foodIds
            .Skip(pageIndex * pageSize)
            .Take(pageSize);
    }

    private List<string> GetCurrentFoodIds()
    {
        return currentFilterFoodRecipeType switch
        {
            FilterFoodRecipeType.Rank1 => rankFoodSortedIds[(int)FilterFoodRecipeType.Rank1],
            FilterFoodRecipeType.Rank2 => rankFoodSortedIds[(int)FilterFoodRecipeType.Rank2],
            FilterFoodRecipeType.Rank3 => rankFoodSortedIds[(int)FilterFoodRecipeType.Rank3],
            _ => defaultFoodSortedIds
        };
    }

    private void UpdateCurrentFoodIds()
    {
        currentCountFoodIds.Value = currentFoodIds.Count;
        currentFoodIdsInPage.Value = GetFoodIds(currentFoodIds, currentPageIndex.CurrentValue, gameConfigDatabase.RecipeCountPerPage).ToArray();
    }
}