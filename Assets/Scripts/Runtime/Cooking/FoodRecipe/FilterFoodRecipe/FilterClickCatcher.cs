using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Cooking
{
    public class FilterClickCatcher : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private LayerMask filterLayerMask;
        [SerializeField] private List<Transform> safeRoots = new();

        private event Action? OnCloseOptionPanel;

        public void SetOnCloseOptionPanel(Action onCloseOptionPanel)
        {
            OnCloseOptionPanel += onCloseOptionPanel;
        }

        public void ClearOnCloseOptionPanel()
        {
            OnCloseOptionPanel = null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count == 0)
            {
                ClosePanel();
                return;
            }

            var hit = results.FirstOrDefault().gameObject;

            if (hit == null || (filterLayerMask.value & (1 << hit.layer)) == 0)
            {
                ClosePanel();
                return;
            }

            if (!IsUnderAnySafeRoot(hit.transform))
            {
                ClosePanel();
            }
        }

        private bool IsUnderAnySafeRoot(Transform target)
        {
            foreach (var root in safeRoots)
            {
                if (root == null)
                {
                    continue;
                }

                if (target == root || target.IsChildOf(root))
                {
                    return true;
                }
            }

            return false;
        }

        private void ClosePanel()
        {
            OnCloseOptionPanel?.Invoke();
        }
    }
}