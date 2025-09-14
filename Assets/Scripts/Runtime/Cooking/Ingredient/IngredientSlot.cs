using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking
{
    public class IngredientSlot : MonoBehaviour
    {
        [SerializeField] private Image ingredientImage = null!;
        [SerializeField] private TextMeshProUGUI ingredientCountTMP = null!;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color emptyColor = Color.red;

        public void SetIngredient(Sprite ingredientSprite, int currentAmount, int requiredAmount)
        {
            ingredientImage.sprite = ingredientSprite;
            SetIngredientCount(currentAmount, requiredAmount);
        }

        public void SetIngredientCount(int currentAmount, int requiredAmount)
        {
            var normalHex = ColorUtility.ToHtmlStringRGB(normalColor);
            var emptyHex = ColorUtility.ToHtmlStringRGB(emptyColor);

            var currentStr = currentAmount == 0
                ? $"<color=#{emptyHex}>{currentAmount}</color>"
                : $"<color=#{normalHex}>{currentAmount}</color>";

            ingredientCountTMP.text = $"{currentStr}/{requiredAmount}";
        }
    }
}