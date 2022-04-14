using Bindito.Core;
using TimberbornAPI.EntityActionSystem;

namespace SunFix
{
    public class RotatingSunConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.MultiBind<IEntityAction>().To<AddComponent>().AsSingleton();
        }

    }
}
