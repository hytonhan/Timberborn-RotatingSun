using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SunFix.UI;
using System;
using System.Linq;
using System.Reflection;
using Timberborn.SkySystem;
using TimberbornAPI;
using TimberbornAPI.Common;

namespace SunFix
{
    [BepInPlugin("hytone.plugins.rotatingsun", "RotatingSunPlugin", "2.0.0")]
    [BepInDependency("com.timberapi.timberapi")]
    [HarmonyPatch]
    public class RotatingSunPlugin : BaseUnityPlugin
    {
        public static bool RotatingSunEnabled { get; set; }
        public static bool RotatingSunFlowersEnabled { get; set; }

        private static Harmony _harmony;
        private static MethodInfo _original;
        public static ConfigFile ConfigFile;
        internal static ManualLogSource Log;

        public void Awake()
        {
            Log = Logger;
            InitConfigs();

            TimberAPI.DependencyRegistry.AddConfigurator(new RotatingSunConfigurator());
            TimberAPI.DependencyRegistry.AddConfigurator(new UIConfigurator(), SceneEntryPoint.Global);

            // Harmony patches
            _original = typeof(Sun).GetMethod(nameof(Sun.RotateSunWithCamera), BindingFlags.NonPublic | BindingFlags.Instance);
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

             if (RotatingSunEnabled)
            {
                var patches = Harmony.GetPatchInfo(original);
                if(patches.Prefixes.Count > 0 
                   && patches.Prefixes.Where(x => x.owner == "hytone.plugins.rotatingsun").Count() > 0)
                {
                    return;
                }
                _harmony.Patch(original, prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.Prefix), new Type[] { typeof(Sun) }));
            }
            else
            {
                _harmony.Unpatch(original, HarmonyPatchType.Prefix, "hytone.plugins.rotatingsun");
            }
        }

        /// <summary>
        /// Enable/Disable Custom Sunflower classes based on the value of RotatingSunFlowersEnabled
        /// </summary>
        public static void SetSunflowerRotation()
        {
            var sunflowers = FindObjectsOfType(typeof(RotatingSunflower));

            foreach(RotatingSunflower sunflower in sunflowers)
            {
                if(RotatingSunFlowersEnabled)
                {
                    sunflower.enabled = true;
                }
                else{
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

        }
    }

}
