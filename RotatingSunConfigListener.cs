
using TimberApi.DependencyContainerSystem;
using Timberborn.SingletonSystem;

namespace SunFix
{
    public class RotatingSunConfigListener : ILoadableSingleton
    {
        private readonly RotatingSunConfig _config;

        public RotatingSunConfigListener(RotatingSunConfig config)
        {
            _config = config;
        }

        public void Load()
        {
            _config.TemperateSunAngleLow.ValueChanged += (object sender, int value) => {
                Patches.XMinAngle = value;
            };
            _config.TemperateSunAngleHigh.ValueChanged += (object sender, int value) => {
                Patches.XMaxAngle = value;
            };
            _config.DroughtSunAngleLow.ValueChanged += (object sender, int value) => {
                Patches.XDroughtMinAngle = value;
            };
            _config.DroughtSunAngleHigh.ValueChanged += (object sender, int value) => {
                Patches.XDroughtMaxAngle = value;
            };
            _config.BadtideSunAngleLow.ValueChanged += (object sender, int value) => {
                Patches.XBadtideMinAngle = value;
            };
            _config.BadtideSunAngleHigh.ValueChanged += (object sender, int value) => {
                Patches.XBadtideMaxAngle = value;
            };
            _config.MoonAngle.ValueChanged += (object sender, int value) => {
                Patches.MoonAngle = value;
            };
            _config.RotatingSunEnabled.ValueChanged += (object sender, bool value) => {
                Patches.SunEnabled = value;
            };
        }
    }
}