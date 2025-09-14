using System;
using System.Collections.Generic;
using Cooking.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking
{
    public class FoodRecipe : MonoBehaviour
    {
        [SerializeField] private GenericButton foodRecipeButton = null!;
        [SerializeField] private Image foodImage = null!;
        [SerializeField] private TextMeshProUGUI foodNameTMP = null!;
        [SerializeField] private List<RectTransform> rankIcons = new();
        [SerializeField] private Sprite selectedSprite = null!;
        [SerializeField] private Sprite unselectedSprite = null!;

        public string? FoodId { get; private set; }

        public void SetClickCallback(Action callback) => foodRecipeButton.SetClickCallback(callback);
        public void ClearClickCallback() => foodRecipeButton.ClearClickCallback();

        public void SetFoodId(string currentFoodId)
        {
            FoodId = currentFoodId;
        }

        public void SetDisplayFoodInfo(string foodName, int rank, Sprite foodSprite)
        {
            for (var i = 0; i < rankIcons.Count; i++)
            {
                rankIcons[i].gameObject.SetActive(i < rank);
            }

            foodImage.sprite = foodSprite;
            foodNameTMP.SetText(foodName);
        }

        public void SetSelected(bool selected)
        {
            foodRecipeButton.SetImage(selected ? selectedSprite : unselectedSprite);
        }
    }
}