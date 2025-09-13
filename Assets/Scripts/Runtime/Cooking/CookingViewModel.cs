using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;

namespace Cooking
{
    public class CookingViewModel : ICookingViewModel, IDisposable
    {
        private readonly ReactiveProperty<CookingState> currentCookingState = new(CookingState.Preparing);
        private readonly CompositeDisposable disposables = new();

        public ReadOnlyReactiveProperty<CookingState> CurrentCookingState => currentCookingState;

        [Inject] private readonly IGameConfigDatabase gameConfigDatabase = null!;
        [Inject] private readonly IFoodDatabase foodDatabase = null!;
        [Inject] private readonly IIngredientDatabase ingredientDatabase = null!;

        public async UniTask StartCookingAsync(CancellationToken cancellationToken)
        {
            //TODO: setup data.

            if (currentCookingState.Value != CookingState.Preparing)
            {
                Debug.LogWarning($"{nameof(CookingViewModel)}: Cooking is not in preparing state");
                return;
            }

            Debug.Log($"{nameof(CookingViewModel)}: Start cooking");
            currentCookingState.Value = CookingState.Cooking;
            await UniTask.Delay(TimeSpan.FromSeconds(3), ignoreTimeScale: true, cancellationToken: cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                //TODO: SaveData
            }

            currentCookingState.Value = CookingState.Preparing;
            Debug.Log($"{nameof(CookingViewModel)}: End cooking");
        }

        public void Dispose()
        {
            currentCookingState.Dispose();
            disposables.Dispose();
            Debug.Log($"{nameof(CookingViewModel)}: Disposed");
        }
    }
}