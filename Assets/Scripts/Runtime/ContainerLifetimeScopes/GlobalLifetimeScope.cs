using Cooking.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Cooking.ContainerLifetimeScopes
{
    public class GlobalLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameConfigDatabase gameConfigDatabase = null!;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<IGameConfigDatabase>(gameConfigDatabase);
            builder.Register<IPauseService, PauseService>(Lifetime.Singleton);
        }
    }
}