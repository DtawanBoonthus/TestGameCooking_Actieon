using Cooking.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Cooking
{
    public class CookingImagePreloader : MonoBehaviour
    {
        [SerializeField] private RectTransform preloadPanel = null!;
        [SerializeField] private string label = "preload_cooking";

        [Inject] private readonly IAddressableImageService imageService = null!;

        private async UniTask Start()
        {
            preloadPanel.gameObject.SetActive(true);
            await imageService.PreloadByLabelAsync(label, destroyCancellationToken);
            preloadPanel.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            imageService.ReleaseAll();
        }
    }
}