using System.Collections.Generic;

namespace Cooking
{
    public interface IFoodDatabase
    {
        IReadOnlyDictionary<string, Food> Foods { get; }
        Food GetById(string foodId);
    }
}