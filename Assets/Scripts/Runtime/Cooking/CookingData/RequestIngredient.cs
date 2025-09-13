using System;
using UnityEngine;

namespace Cooking.CookingData;

[Serializable]
public record RequestIngredient
{
    [SerializeField] private string ingredientId = string.Empty;
    [SerializeField] private int amount;

    public string IngredientId => ingredientId;
    public int Amount => amount;
}