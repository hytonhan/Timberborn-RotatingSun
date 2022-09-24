using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SunFix.UI;
using System;
using System.Linq;
using System.Reflection;
using TimberApi.ModSystem;
using TimberApi.ConsoleSystem;
using Timberborn.SkySystem;

namespace SunFix
{
    [BepInPlugin("hytone.plugins.rotatingsun", "RotatingSunPlugin", "3.0.0")]
    [HarmonyPatch]
    public class RotatingSunPlugin : BaseUnityPlugin, IModEntrypoint
    {
        public static bool RotatingSunEnabled { get; set; }
        public static bool RotatingSunFlowersEnabled { get; set; }

        public static int TemperateSunAngleLow { get; set; }
        public static int TemperateSunAngleHigh { get; set; }
        public static int DroughtSunAngleLow { get; set; }
        public static int DroughtSunAngleHigh { get; set; }

        public static int MoonAngle { get; set; }


        private static Harmony _harmony;
        public static ConfigFile ConfigFile;
        internal static ManualLogSource Log;

        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            InitConfigs();

            _harmony = new Harmony("hytone.plugins.rotatingsun");
            _harmony.PatchAll();
            PatchSunRotation();

            ConfigFile = Config;
        }

        /// <summary>
        /// Patches or Unpatches the Sun Rotation based on value of RotatingSunEnabled
        /// </summary>
        public static void PatchSunRotation()
        {
            var original = typeof(Sun).GetMethod(nameof(Sun.RotateSunWithCamera), BindingFlags.NonPublic | BindingFlags.Instance);
            var original2 = typeof(Sun).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                       .Single(x => x.Name == nameof(Sun.UpdateColors) &&
                                                    x.IsPrivate == true &&
                                                    x.GetParameters().Length == 1);

            if (RotatingSunEnabled)
            {
                var patches = Harmony.GetPatchInfo(original);
                if (patches.Prefixes.Count == 0
                    || patches.Prefixes.Where(x => x.owner == "hytone.plugins.rotatingsun").Count() == 0)
                {
                    _harmony.Patch(original,
                                   prefix: new HarmonyMethod(typeof(Patches),
                                                             nameof(Patches.Prefix),
                                                             new Type[] { typeof(Sun), typeof(DayStageTransition) }));
                }
                patches = Harmony.GetPatchInfo(original2);
                if (patches.Prefixes.Count == 0
                    || patches.Prefixes.Where(x => x.owner == "hytone.plugins.rotatingsun").Count() == 0)
                {
                    _harmony.Patch(original2,
                                   prefix: new HarmonyMethod(typeof(Patches),
                                                             nameof(Patches.UpdateColorsPrefix),
                                                             new Type[] { typeof(Sun), typeof(DayStageTransition) }));
                }
            }
            else
            {
                _harmony.Unpatch(original, HarmonyPatchType.Prefix, "hytone.plugins.rotatingsun");
                _harmony.Unpatch(original2, HarmonyPatchType.Prefix, "hytone.plugins.rotatingsun");
            }
        }

        /// <summary>
        /// Enable/Disable Custom Sunflower classes based on the value of RotatingSunFlowersEnabled
        /// </summary>
        public static void SetSunflowerRotation()
        {
            var sunflowers = FindObjectsOfType(typeof(RotatingSunflower));

            foreach (RotatingSunflower sunflower in sunflowers.Where(x => x.name.Contains("Sun")))
            {
                if (RotatingSunFlowersEnabled)
                {
                    sunflower.enabled = true;
                }
                else
                {
                    sunflower.enabled = false;
                }
            }
        }

        private void InitConfigs()
        {
            RotatingSunEnabled = Config.Bind(
                "General",
                nameof(RotatingSunEnabled),
                true,
                "Enable the Sun to rotate around the world instead of being tied to player camera.").Value;

            RotatingSunFlowersEnabled = Config.Bind(
                "General",
                nameof(RotatingSunFlowersEnabled),
                false,
                "Enable Sunflowers to rotate to face the Sun.").Value;

            TemperateSunAngleLow = Config.Bind(
                "General",
                nameof(TemperateSunAngleLow),
                SunMenu._sunAngleMinDefault,
                "Sun starting angle during Temperate weather.").Value;

            TemperateSunAngleHigh = Config.Bind(
                "General",
                nameof(TemperateSunAngleHigh),
                SunMenu._sunAngleMaxDefaultTemperate,
                "Sun High angle during Temperate weather.").Value;

            DroughtSunAngleLow = Config.Bind(
                "General",
                nameof(DroughtSunAngleLow),
                SunMenu._sunAngleMinDefault,
                "Sun starting angle during Drought.").Value;

            DroughtSunAngleHigh = Config.Bind(
                "General",
                nameof(DroughtSunAngleHigh),
                SunMenu._sunAngleMaxDefaultDrought,
                "Sun High angle during Drought.").Value;

            MoonAngle = Config.Bind(
                "General",
                nameof(MoonAngle),
                SunMenu._moonAngleDefault,
                "The Moon's angle.").Value;

        }

    }

}
