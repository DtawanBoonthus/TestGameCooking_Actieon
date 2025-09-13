using System;
using UnityEngine;

namespace Cooking;

[Serializable]
public record Ingredient
{
    [SerializeField] private string id = string.Empty;
    [SerializeField] private string name = string.Empty;
    [SerializeField] private string imageName = string.Empty;

    public string Id => id;
    public string Name => name;
    public string ImageName => imageName;
}