using Cooking.Services;
using Cooking.Utilities;
using UnityEngine;
using VContainer;

namespace Cooking
{
    public class CookingPauseHandler : MonoBehaviour
    {
        [SerializeField] private GenericButton pauseButton = null!;
        [SerializeField] private GenericButton resumeButton = null!;
        [Space] [SerializeField] private RectTransform pauseMenu = null!;

        [Inject] private readonly IPauseService pauseService = null!;

        private void OnEnable()
        {
            pauseButton.SetClickCallback(OnPauseButtonClicked);
            resumeButton.SetClickCallback(OnResumeButtonClicked);
        }

        private void OnDisable()
        {
            pauseButton.ClearClickCallback();
            resumeButton.ClearClickCallback();
        }

        private void OnPauseButtonClicked()
        {
            pauseMenu.gameObject.SetActive(true);
            pauseService.Pause();
        }

        private void OnResumeButtonClicked()
        {
            pauseService.Resume();
            pauseMenu.gameObject.SetActive(false);
        }
    }
}