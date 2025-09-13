using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cooking.CookingData;

[Serializable]
public class Food
{
    [SerializeField] private string id = string.Empty;
    [SerializeField] private string name = string.Empty;
    [SerializeField] private int rank;
    [SerializeField] private string imageName = string.Empty;
    [SerializeField] private List<RequestIngredient> ingredients = new();
    
    public string Id => id;
    public string Name => name;
    public int Rank => rank;
    public string ImageName => imageName;
    public IReadOnlyList<RequestIngredient> Ingredients => ingredients;
}