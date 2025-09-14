using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cooking.Services;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Cooking
{
    public class CookingViewModel : ICookingViewModel, IAsyncStartable, IDisposable
    {
        private readonly ReactiveProperty<bool> canCooking = new(false);
        private readonly ReactiveProperty<TimeSpan> cookingTime = new();
        private readonly ReactiveProperty<float> currentEnergyNormalize = new(0);
        private readonly ReactiveProperty<Food?> gainFood = new();
        private readonly ReactiveProperty<CookingState> currentCookingState = new(CookingState.Preparing);

        public bool IsImagePreloaded { get; set; } = false;
        public ReadOnlyReactiveProperty<bool> CanCooking => canCooking;
        public ReadOnlyReactiveProperty<TimeSpan> CookingTime => cookingTime;
        public ReadOnlyReactiveProperty<CookingState> CurrentCookingState => currentCookingState;
        public ReadOnlyReactiveProperty<float> CurrentEnergyNormalize => currentEnergyNormalize;
        public int MaxEnergy => gameConfigDatabase.MaxCookingEnergy;
        public int CurrentEnergy => playerData.Energy.CurrentValue;
        public ReadOnlyReactiveProperty<Food?> GainFood => gainFood;

        [Inject] private readonly IGameConfigDatabase gameConfigDatabase = null!;
        [Inject] private readonly IFoodDatabase foodDatabase = null!;
        [Inject] private readonly IPlayerData playerData = null!;
        [Inject] private readonly IFoodRecipeViewModel foodRecipeViewModel = null!;
        [Inject] private readonly ISaveLoadService saveLoadService = null!;

        private readonly CompositeDisposable disposables = new();

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await UniTask.WaitUntil(() => IsImagePreloaded, cancellationToken: cancellation);
            LoadPendingCookingAsync(cancellation).Forget();

            playerData.Energy.Subscribe(energy =>
            {
                var maxCookingEnergy = gameConfigDatabase.MaxCookingEnergy;
                var energyNormalize = Mathf.Clamp01(energy / (float)maxCookingEnergy);
                currentEnergyNormalize.Value = energyNormalize;
            }).AddTo(disposables);
            foodRecipeViewModel.CurrentFoodId.Subscribe(_ => ValidateCookingReadiness(currentCookingState.CurrentValue)).AddTo(disposables);
            currentCookingState.Subscribe(ValidateCookingReadiness).AddTo(disposables);
        }

        public void Dispose()
        {
            disposables.Dispose();
            currentCookingState.Dispose();
            Debug.Log($"{nameof(CookingViewModel)}: Disposed");
        }

        public async UniTask StartCookingAsync(CancellationToken cancellationToken)
        {
            if (currentCookingState.Value != CookingState.Preparing)
            {
                Debug.LogWarning($"{nameof(CookingViewModel)}: Cooking is not in preparing state");
                return;
            }

            Debug.Log($"{nameof(CookingViewModel)}: Cooking started");
            currentCookingState.Value = CookingState.Cooking;
            var food = foodDatabase.GetById(foodRecipeViewModel.CurrentFoodId.CurrentValue);
            try
            {
                playerData.DecreaseIngredient(food.Ingredients.ToDictionary(x => x.IngredientId, x => x.Amount));
                playerData.DecreaseEnergy(gameConfigDatabase.CookingEnergyCost);
                saveLoadService.Save(SaveLoadKey.PLAYER_DATA, playerData);
                saveLoadService.Save(SaveLoadKey.PENDING_COOKING, new PendingCooking
                {
                    FoodId = food.Id,
                    FinishTime = DateTime.UtcNow.AddSeconds(food.CookingTimeSecond)
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"{ex.Message}");
                throw;
            }

            cookingTime.Value = TimeSpan.FromSeconds(food.CookingTimeSecond);
            await UniTask.Delay(TimeSpan.FromSeconds(food.CookingTimeSecond), ignoreTimeScale: true, cancellationToken: cancellationToken);
            cookingTime.Value = TimeSpan.Zero;
            saveLoadService.Save(SaveLoadKey.PENDING_COOKING, null);
            gainFood.Value = food;
            currentCookingState.Value = CookingState.Succeeded;
            Debug.Log($"{nameof(CookingViewModel)}: Cooking finished");
        }

        public void OnGainedFood()
        {
            gainFood.Value = null;
            currentCookingState.Value = CookingState.Preparing;
        }

        private void ValidateCookingReadiness(CookingState state)
        {
            if (playerData.Energy.CurrentValue < gameConfigDatabase.CookingEnergyCost)
            {
                canCooking.Value = false;
                return;
            }

            var foodId = foodRecipeViewModel.CurrentFoodId.CurrentValue;

            if (string.IsNullOrWhiteSpace(foodId))
            {
                canCooking.Value = false;
                return;
            }

            var food = foodDatabase.GetById(foodId);
            var canStartCooking = ValidateCanCooking(food.Ingredients, playerData.Ingredients.CurrentValue);

            canCooking.Value = state == CookingState.Preparing && canStartCooking;
        }

        private static bool ValidateCanCooking(IReadOnlyList<RequestIngredient> requestIngredients, IReadOnlyDictionary<string, int> playerIngredients)
        {
            foreach (var requestIngredient in requestIngredients)
            {
                if (!playerIngredients.TryGetValue(requestIngredient.IngredientId, out var count) || count < requestIngredient.Amount)
                {
                    return false;
                }
            }

            return true;
        }

        private UniTask LoadPendingCookingAsync(CancellationToken cancellation)
        {
            var pending = saveLoadService.Load<PendingCooking>(SaveLoadKey.PENDING_COOKING);

            if (pending == null)
            {
                return UniTask.CompletedTask;
            }

            var now = DateTime.UtcNow;
            var finish = pending.FinishTime;

            if (finish <= now)
            {
                var food = foodDatabase.GetById(pending.FoodId);
                gainFood.Value = food;
                currentCookingState.Value = CookingState.Succeeded;
                saveLoadService.Save(SaveLoadKey.PENDING_COOKING, null);
                return UniTask.CompletedTask;
            }

            currentCookingState.Value = CookingState.Cooking;
            ContinueCookingAsync(pending.FoodId, finish - now, cancellation).Forget();
            return UniTask.CompletedTask;
        }

        private async UniTask ContinueCookingAsync(string foodId, TimeSpan remaining, CancellationToken cancellationToken)
        {
            saveLoadService.Save(SaveLoadKey.PENDING_COOKING, new PendingCooking
            {
                FoodId = foodId,
                FinishTime = DateTime.UtcNow.Add(remaining)
            });
            cookingTime.Value = remaining;
            await UniTask.Delay(remaining, cancellationToken: cancellationToken, ignoreTimeScale: true);
            cookingTime.Value = TimeSpan.Zero;
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var food = foodDatabase.GetById(foodId);
            gainFood.Value = food;
            currentCookingState.Value = CookingState.Succeeded;
            saveLoadService.Save(SaveLoadKey.PENDING_COOKING, null);
        }
    }
}