using Cooking.Utilities;
using R3;
using UnityEngine;
using VContainer;

namespace Cooking
{
    public class CookingPauseView : MonoBehaviour
    {
        [SerializeField] private GenericButton pauseButton = null!;
        [SerializeField] private GenericButton resumeButton = null!;
        [SerializeField] private RectTransform pausePanel = null!;

        [Inject] private readonly ICookingPauseViewModel cookingPauseViewModel = null!;

        private readonly CompositeDisposable disposables = new();

        private void Start()
        {
            pauseButton.SetClickCallback(cookingPauseViewModel.Pause);
            resumeButton.SetClickCallback(cookingPauseViewModel.Resume);
            BindPausePanelVisibility();
        }

        private void OnDestroy()
        {
            pauseButton.ClearClickCallback();
            resumeButton.ClearClickCallback();
            disposables.Dispose();
        }

        private void BindPausePanelVisibility()
        {
            cookingPauseViewModel.IsShowPausePanel
                .Subscribe(isShow => pausePanel.gameObject.SetActive(isShow))
                .AddTo(disposables);
        }
    }
}