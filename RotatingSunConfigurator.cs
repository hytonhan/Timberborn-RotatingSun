using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.Fields;
using Timberborn.TemplateSystem;

namespace SunFix
{
    [Configurator(SceneEntrypoint.InGame)]
    public class RotatingSunConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<Crop, RotatingSunflower>();
            return builder.Build();
        }

    }
}
