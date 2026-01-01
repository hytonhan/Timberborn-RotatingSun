using HarmonyLib;
using Timberborn.ModManagerScene;

namespace SunFix
{
    public class RotatingSunPlugin : IModStarter
    {
        public static Harmony HarmonyInstance;

        public void StartMod(IModEnvironment modEnvironment)
        {
            HarmonyInstance = new Harmony("hytone.plugins.rotatingsun");
            HarmonyInstance.PatchAll();
        }
    }
}
