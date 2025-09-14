using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cooking;

[Serializable]
public record Food
{
    [SerializeField] private string id = string.Empty;
    [SerializeField] private string name = string.Empty;
    [SerializeField] private int rank;
    [SerializeField] private string imageName = string.Empty;
    [SerializeField] private List<RequestIngredient> ingredients = new();
    [SerializeField] private float cookingTimeSecond = 10f;

    public string Id => id;
    public string Name => name;
    public int Rank => rank;
    public string ImageName => imageName;
    public IReadOnlyList<RequestIngredient> Ingredients => ingredients;
    public float CookingTimeSecond => cookingTimeSecond;
}