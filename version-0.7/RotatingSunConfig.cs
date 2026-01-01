using ModSettings.Core;
using Timberborn.Modding;
using Timberborn.SettingsSystem;

namespace SunFix
{
    public class RotatingSunConfig : ModSettingsOwner
    {

        public RotatingSunConfig(
            ISettings settings, 
            ModSettingsOwnerRegistry modSettingsOwnerRegistry, 
            ModRepository modRepository) 
            : base(settings, modSettingsOwnerRegistry, modRepository)
        {
        }

        public ModSetting<bool> RotatingSunEnabled {get; } = 
            new (true, ModSettingDescriptor.CreateLocalized("rotatingsun.enablesun"));

        public ModSetting<bool> RotatingSunFlowersEnabled {get; } = 
            new (false, ModSettingDescriptor.CreateLocalized("rotatingsun.enablerotatingsunflowers"));

        public ModSetting<int> TemperateSunAngleLow {get; } = 
            new (5, ModSettingDescriptor.CreateLocalized("rotatingsun.SunAngleLowTemperate"));

        public ModSetting<int> TemperateSunAngleHigh {get; } = 
            new (50, ModSettingDescriptor.CreateLocalized("rotatingsun.SunAngleHighTemperate"));

        public ModSetting<int> DroughtSunAngleLow {get; } = 
            new (5, ModSettingDescriptor.CreateLocalized("rotatingsun.SunAngleLowDrought"));

        public ModSetting<int> DroughtSunAngleHigh {get; } = 
            new (80, ModSettingDescriptor.CreateLocalized("rotatingsun.SunAngleHighDrought"));

        public ModSetting<int> BadtideSunAngleLow {get; } = 
            new (5, ModSettingDescriptor.CreateLocalized("rotatingsun.SunAngleLowBadtide"));

        public ModSetting<int> BadtideSunAngleHigh {get; } = 
            new (80, ModSettingDescriptor.CreateLocalized("rotatingsun.SunAngleHighBadtide"));

        public ModSetting<int> MoonAngle {get; } = 
            new (50, ModSettingDescriptor.CreateLocalized("rotatingsun.MoonAngle"));

        public override string HeaderLocKey => "menu.rotatingsun";
        
        protected override string ModId => "hytone.rotatingsun";

    }
}
