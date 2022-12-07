using HarmonyLib;
using SunFix.UI;
using System;
using System.Linq;
using System.Reflection;
using TimberApi.ModSystem;
using TimberApi.ConsoleSystem;
using Timberborn.SkySystem;
using UnityEngine;

namespace SunFix
{
    [HarmonyPatch]
    public class RotatingSunPlugin : IModEntrypoint
    {
        private static Harmony _harmony;
        public static RotatingSunConfig Config;

        public static IConsoleWriter Log;

        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            Config = mod.Configs.Get<RotatingSunConfig>();

            _harmony = new Harmony("hytone.plugins.rotatingsun");
            _harmony.PatchAll();
            PatchSunRotation();

            Log = consoleWriter;
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

            if (Config.RotatingSunEnabled)
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
            var sunflowers = UnityEngine.Object.FindObjectsOfType(typeof(RotatingSunflower));

            foreach (RotatingSunflower sunflower in sunflowers.Where(x => x.name.Contains("Sun")))
            {
                if (Config.RotatingSunFlowersEnabled)
                {
                    sunflower.enabled = true;
                }
                else
                {
                    sunflower.enabled = false;
                }
            }
        }
    }
}
