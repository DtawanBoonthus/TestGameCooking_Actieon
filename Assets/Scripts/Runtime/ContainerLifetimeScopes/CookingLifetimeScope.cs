using System;
using Cooking.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Cooking.ContainerLifetimeScopes
{
    public class CookingLifetimeScope : LifetimeScope
    {
        [SerializeField] private FoodDatabase foodDatabase = null!;
        [SerializeField] private IngredientDatabase ingredientDatabase = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IAddressableImageService, AddressableImageService>(Lifetime.Scoped);
            builder.RegisterInstance<IFoodDatabase>(foodDatabase);
            builder.RegisterInstance<IIngredientDatabase>(ingredientDatabase);
            builder.Register<ICookingPauseViewModel, CookingPauseViewModel>(Lifetime.Scoped);
            builder.Register<ICookingViewModel, CookingViewModel>(Lifetime.Scoped).As<IAsyncStartable, IDisposable>();
            builder.Register<IFoodRecipeViewModel, FoodRecipeViewModel>(Lifetime.Scoped);
        }
    }
}