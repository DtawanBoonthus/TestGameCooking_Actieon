using Cooking.Services;
using VContainer;
using VContainer.Unity;

namespace Cooking.ContainerLifetimeScopes
{
    public class GlobalLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IPauseService, PauseService>(Lifetime.Scoped);
        }
    }
}