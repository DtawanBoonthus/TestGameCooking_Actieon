using Cooking.Utilities;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
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

        [Inject] protected readonly ICookingViewModel cookingViewModel = null!;
        private readonly CompositeDisposable disposables = new();

        private void OnEnable()
        {
            cookingButton.SetClickCallback(OnCookingButtonClicked);
            closeButton.SetClickCallback(OnCloseButtonClicked);
            startCookingButton.SetClickCallback(OnStartCookingButtonClicked);
        }

        private void Start()
        {
            cookingViewModel.CurrentCookingState.Subscribe(OnCookingStatusChange).AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Dispose();
            cookingButton.ClearClickCallback();
            closeButton.ClearClickCallback();
            startCookingButton.ClearClickCallback();
            Debug.Log($"{nameof(CookingPanelView)}: Disposed");
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

        private void OnCookingStatusChange(CookingState cookingState)
        {
            var isInteractable = cookingState switch
            {
                CookingState.Preparing => true,
                _ => false
            };

            startCookingButton.SetInteractable(isInteractable);
        }
    }
}