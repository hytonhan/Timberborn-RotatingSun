using Bindito.Core;

namespace SunFix.UI
{
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<SunMenu>().AsSingleton();
        }
    }
}
