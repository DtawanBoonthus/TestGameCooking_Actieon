namespace Cooking;

public interface IGameConfigDatabase
{
    int RecipeCountPerPage { get; }
    int SearchCutoffScore { get; }
}