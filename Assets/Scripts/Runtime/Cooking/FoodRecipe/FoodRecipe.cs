using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking
{
    public class FoodRecipe : MonoBehaviour
    {
        [SerializeField] private Image foodImage = null!;
        [SerializeField] private TextMeshProUGUI foodNameTMP = null!;
        [SerializeField] private List<RectTransform> rankIcons = new();

        public void SetDisplayFoodInfo(string foodName, int rank, Sprite foodSprite)
        {
            for (var i = 0; i < rankIcons.Count; i++)
            {
                rankIcons[i].gameObject.SetActive(i < rank);
            }

            foodImage.sprite = foodSprite;
            foodNameTMP.SetText(foodName);
        }
    }
}