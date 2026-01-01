using Bindito.Core;
using Timberborn.Fields;
using Timberborn.TemplateInstantiation;
using Timberborn.TemplateSystem;

namespace SunFix
{  
    [Context("Game")]
    [Context("MapEditor")]
    public class RotatingSunConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
            containerDefinition.Bind<RotatingSunConfig>().AsSingleton();
            containerDefinition.Bind<Patches>().AsSingleton();
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<Crop, RotatingSunflower>();
            return builder.Build();
        }

    }

    [Context("MainMenu")]
    public class RotatingSunConfigurator2 : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<RotatingSunConfigListener>().AsSingleton();
            containerDefinition.Bind<RotatingSunConfig>().AsSingleton();
        }

    }
}
