using System;
using System.Threading;
using Cooking.Services;
using Cooking.Utilities;
using Cysharp.Threading.Tasks;
using R3;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Cooking
{
    public class CookingPanelView : MonoBehaviour
    {
        [SerializeField] private GenericButton cookingButton = null!;
        [SerializeField] private GenericButton closeButton = null!;
        [SerializeField] private RectTransform cookingPanel = null!;
        [SerializeField] private RectTransform dimPanel = null!;
        [SerializeField] private GenericButton startCookingButton = null!;
        [SerializeField] private Slider energySlider = null!;
        [SerializeField] private TextMeshProUGUI energyTMP = null!;
        [SerializeField] private SkeletonGraphic potSpine = null!;
        [SerializeField] private RectTransform gainFoodPanel = null!;
        [SerializeField] private TextMeshProUGUI gainFoodTMP = null!;
        [SerializeField] private GenericButton gainFoodButton = null!;
        [SerializeField] private Image gainFoodImage = null!;
        [SerializeField] private TextMeshProUGUI cookingTimeTMP = null!;

        [SerializeField] private AnimationReferenceAsset idleAnimation = null!;
        [SerializeField] private AnimationReferenceAsset cookingAnimation = null!;
        [SerializeField] private AnimationReferenceAsset succeededAnimation = null!;
        [SerializeField] private AnimationReferenceAsset idleSucceededAnimation = null!;

        [Inject] private readonly IAddressableImageService imageService = null!;
        [Inject] private readonly ICookingViewModel cookingViewModel = null!;

        private readonly CompositeDisposable disposables = new();
        private CancellationTokenSource? countdownCts;

        private void Start()
        {
            gainFoodButton.SetClickCallback(OnGainButtonClicked);
            cookingButton.SetClickCallback(OnCookingButtonClicked);
            closeButton.SetClickCallback(OnCloseButtonClicked);
            startCookingButton.SetClickCallback(OnStartCookingButtonClicked);

            cookingViewModel.CookingTime.Subscribe(OnCookingTimeChanged).AddTo(disposables);
            cookingViewModel.GainFood.Subscribe(GainFoodChanged).AddTo(disposables);
            cookingViewModel.CurrentEnergyNormalize.Subscribe(OnEnergyChanged).AddTo(disposables);
            cookingViewModel.CurrentCookingState.Subscribe(OnCookingStatusChange).AddTo(disposables);
            cookingViewModel.CanCooking.Subscribe(canCooking => startCookingButton.SetInteractable(canCooking)).AddTo(disposables);
        }

        private void OnDestroy()
        {
            gainFoodButton.ClearClickCallback();
            disposables.Dispose();
            cookingButton.ClearClickCallback();
            closeButton.ClearClickCallback();
            startCookingButton.ClearClickCallback();

            countdownCts?.Cancel();
            countdownCts?.Dispose();
            countdownCts = null;
            Debug.Log($"{nameof(CookingPanelView)}: Disposed");
        }

        private void GainFoodChanged(Food? food)
        {
            if (food == null)
            {
                return;
            }

            gainFoodPanel.gameObject.SetActive(true);
            gainFoodTMP.SetText($"Gain {food.Name}");
            gainFoodImage.sprite = imageService.TryGetFromCache(food.ImageName);
        }

        private void OnGainButtonClicked()
        {
            gainFoodPanel.gameObject.SetActive(false);
            cookingViewModel.OnGainedFood();
        }

        private void OnCookingButtonClicked()
        {
            dimPanel.gameObject.SetActive(true);
            cookingPanel.gameObject.SetActive(true);
        }

        private void OnCloseButtonClicked()
        {
            dimPanel.gameObject.SetActive(false);
            cookingPanel.gameObject.SetActive(false);
        }

        private void OnStartCookingButtonClicked()
        {
            cookingViewModel.StartCookingAsync(destroyCancellationToken).Forget();
        }

        private void OnEnergyChanged(float energyNormalize)
        {
            energySlider.value = energyNormalize;
            energyTMP.text = $"{cookingViewModel.CurrentEnergy}/{cookingViewModel.MaxEnergy}";
        }

        private void OnCookingStatusChange(CookingState cookingState)
        {
            switch (cookingState)
            {
                case CookingState.Preparing:
                    SetAnim(idleAnimation);
                    break;
                case CookingState.Cooking:
                    SetAnim(cookingAnimation);
                    break;
                case CookingState.Succeeded:
                    SetAnim(succeededAnimation, false);
                    QueueAnim(idleSucceededAnimation, true, succeededAnimation.Animation.Duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cookingState), cookingState, null);
            }
        }

        private void SetAnim(AnimationReferenceAsset animRef, bool loop = true)
        {
            if (animRef == null || animRef.Animation == null)
            {
                return;
            }

            var state = potSpine.AnimationState;
            state.SetAnimation(0, animRef, loop);
        }

        private void QueueAnim(AnimationReferenceAsset animRef, bool loop, float delay)
        {
            if (animRef == null || animRef.Animation == null) return;

            var state = potSpine.AnimationState;
            state.AddAnimation(0, animRef, loop, delay);
        }

        private void OnCookingTimeChanged(TimeSpan cookingTime)
        {
            StopCountdown();

            if (cookingTime <= TimeSpan.Zero)
            {
                cookingTimeTMP.SetText(FormatMinSecCeil(cookingTime));
                return;
            }

            cookingTimeTMP.SetText(FormatMinSecCeil(cookingTime));
            countdownCts = new CancellationTokenSource();
            CountdownAsync(cookingTime, countdownCts.Token).Forget();
        }

        private void StopCountdown()
        {
            countdownCts?.Cancel();
            countdownCts?.Dispose();
            countdownCts = null;
        }

        private async UniTaskVoid CountdownAsync(TimeSpan startTime, CancellationToken cancellationToken)
        {
            var remaining = startTime;

            while (remaining > TimeSpan.Zero && !cancellationToken.IsCancellationRequested)
            {
                cookingTimeTMP.SetText(FormatMinSecCeil(remaining));


                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                remaining -= TimeSpan.FromSeconds(Time.unscaledDeltaTime);

                if (remaining < TimeSpan.Zero)
                {
                    remaining = TimeSpan.Zero;
                }
            }

            cookingTimeTMP.SetText(FormatMinSecCeil(remaining));
        }

        private static string FormatMinSecCeil(TimeSpan timeSpan)
        {
            if (timeSpan < TimeSpan.Zero)
            {
                timeSpan = TimeSpan.Zero;
            }

            var totalSeconds = (int)Math.Ceiling(timeSpan.TotalSeconds);

            var m = totalSeconds / 60;
            var s = totalSeconds % 60;
            return $"{m}:{s:00}";
        }
    }
}