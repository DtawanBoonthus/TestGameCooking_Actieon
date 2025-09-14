using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Cooking.Utilities
{
    public class GenericButton : MonoBehaviour
    {
        [SerializeField] protected Button button = null!;
        [SerializeField] protected Image image = null!;
        [SerializeField] protected TextMeshProUGUI? textTMP;

        protected event Action? clickCallback;

        protected virtual void OnEnable()
        {
            button.onClick.AddListener(OnButtonClickedWrapper);
        }

        protected virtual void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClickedWrapper);
        }

        protected virtual void OnButtonClickedWrapper()
        {
            clickCallback?.Invoke();
        }

        public virtual void SetClickCallback(Action callback) => clickCallback = callback;
        public virtual void ClearClickCallback() => clickCallback = null;
        public virtual void SetImage(Sprite sprite) => image.sprite = sprite;
        public virtual void SetInteractable(bool interactable) => button.interactable = interactable;

        public virtual void SetText(string text)
        {
            if (textTMP == null)
            {
                Debug.LogWarning("TextTMP is null");
            }

            textTMP?.SetText(text);
        }
    }
}