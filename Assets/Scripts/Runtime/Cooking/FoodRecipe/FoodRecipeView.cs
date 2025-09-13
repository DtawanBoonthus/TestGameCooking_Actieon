using System;
using Cooking.Utilities;
using UnityEngine;

namespace Cooking
{
    public class FoodRecipeView : MonoBehaviour
    {
        [SerializeField] private GenericButton leftButton = null!;
        [SerializeField] private GenericButton rightButton = null!;
        [SerializeField] private GameObject pagePrefab = null!;
        
        private void OnEnable()
        {
            throw new NotImplementedException();
        }

        private void OnDisable()
        {
            throw new NotImplementedException();
        }
    }
}