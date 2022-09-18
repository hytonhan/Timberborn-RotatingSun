using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;

namespace SunFix.UI
{
    [Configurator(SceneEntrypoint.InGame | SceneEntrypoint.MainMenu)]
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<SunMenu>().AsSingleton();
        }
    }
}
