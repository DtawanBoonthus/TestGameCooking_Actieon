using System.Collections.Generic;

namespace Cooking.CookingData
{
    public interface IFoodDatabase
    {
        IReadOnlyDictionary<string, Food> Foods { get; }
        Food GetById(string foodId);
    }
}