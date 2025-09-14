using System;
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

        [SerializeField] private AnimationReferenceAsset idleAnimation = null!;
        [SerializeField] private AnimationReferenceAsset cookingAnimation = null!;
        [SerializeField] private AnimationReferenceAsset succeededAnimation = null!;
        [SerializeField] private AnimationReferenceAsset idleSucceededAnimation = null!;

        [Inject] private readonly IAddressableImageService imageService = null!;
        [Inject] private readonly ICookingViewModel cookingViewModel = null!;

        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            gainFoodButton.SetClickCallback(OnGainButtonClicked);
            cookingButton.SetClickCallback(OnCookingButtonClicked);
            closeButton.SetClickCallback(OnCloseButtonClicked);
            startCookingButton.SetClickCallback(OnStartCookingButtonClicked);
            
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
    }
}