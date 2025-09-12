using VContainer;
using VContainer.Unity;

namespace Cooking.ContainerLifetimeScopes
{
    public class CookingLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
        }
    }
}