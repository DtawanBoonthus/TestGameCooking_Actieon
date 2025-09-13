using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Cooking
{
    public interface ICookingViewModel
    {
        ReadOnlyReactiveProperty<CookingState> CurrentCookingState { get; }
        ReadOnlyReactiveProperty<int> FoodCount { get; }
        UniTask StartCookingAsync(CancellationToken cancellationToken);
    }
}