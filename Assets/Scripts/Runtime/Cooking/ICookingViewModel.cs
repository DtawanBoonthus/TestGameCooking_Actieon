using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace Cooking
{
    public interface ICookingViewModel
    {
        bool IsImagePreloaded { get; set; }
        ReadOnlyReactiveProperty<bool> CanCooking { get; }
        ReadOnlyReactiveProperty<CookingState> CurrentCookingState { get; }
        ReadOnlyReactiveProperty<float> CurrentEnergyNormalize { get; }
        int MaxEnergy { get; }
        int CurrentEnergy { get; }
        UniTask StartCookingAsync(CancellationToken cancellationToken);
        ReadOnlyReactiveProperty<Food?> GainFood { get; }
        void OnGainedFood();
    }
}