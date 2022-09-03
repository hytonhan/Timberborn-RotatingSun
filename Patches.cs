using HarmonyLib;
using SunFix.UI;
using System;
using Timberborn.Localization;
using Timberborn.MainMenuScene;
using Timberborn.Options;
using Timberborn.SkySystem;
using TimberbornAPI;
using UnityEngine;
using UnityEngine.UIElements;

namespace SunFix
{

    [HarmonyPatch]
    public static class Patches
    {
        public static float XMinAngle = 5f;
        public static float XMaxAngle = 50f;
        public static float XDroughtMinAngle = 5f;
        public static float XDroughtMaxAngle = 80f;
        private static float _transitionProgress = 0f;
        private static float _transitionTime = 0.3f; // A Tick lasts 0.3 s
        private static float _lastTimestamp;
        private static float _x;
        private static float _y;
        private static Quaternion _lastRotation;

        [HarmonyPatch(typeof(Sun), nameof(Sun.RotateSunWithCamera))]
        public static bool Prefix(Sun __instance)
        {
            var hoursToday = __instance._dayStageCycle._dayNightCycle.HoursPassedToday;
            var dayLength = __instance._dayStageCycle._dayNightCycle.DaytimeLengthInHours;
            var nightLength = __instance._dayStageCycle._dayNightCycle.NighttimeLengthInHours;

            var progress = (hoursToday / dayLength);

            _transitionProgress += Time.deltaTime;
            float cappedTransitionPorgress = Math.Min(1f, _transitionProgress / _transitionTime);

            // Time in DayNightCycle is calculated in ticks. Calculate new rotation angles when 
            // a new tick is detected
            if (_lastTimestamp != hoursToday)
            {
                _transitionProgress = 0;
                _lastRotation = __instance._sun.transform.localRotation;
                _lastTimestamp = hoursToday;

                // This if calculates x and y angles during daytime.
                // x is basically shadow's length.
                // y is the sun's rotation.
                // During day time, x goes 0 -> 90 -> 0 and y 0 -> 180, with little offsets
                if (progress <= 1)
                {
                    if (progress <= 0.5)
                    {
                        // in the morning sun goes up, maxing at _xOffset
                        if (__instance._dayStageCycle._weatherService._droughtService.IsDrought)
                        {
                            _x = Mathf.Clamp(progress * (XDroughtMaxAngle * 2) + XDroughtMinAngle, XDroughtMinAngle, XDroughtMaxAngle);
                        }
                        else {
                            _x = Mathf.Clamp(progress * (XMaxAngle * 2) + XMinAngle, XMinAngle, XMaxAngle);
                        }
                    }
                    else
                    {
                        // in the afternoon sun goes down
                        if (__instance._dayStageCycle._weatherService._droughtService.IsDrought)
                        {
                            _x = Mathf.Clamp((XDroughtMaxAngle * 2) - ((progress) * (XDroughtMaxAngle * 2)), 0f, XDroughtMaxAngle);
                        }
                        else 
                        {
                             _x = Mathf.Clamp((XMaxAngle * 2) - ((progress) * (XMaxAngle * 2)), 0f, XMaxAngle);
                        }
                    }
                    // During day, y goes smoothly from 10 to 170
                    _y = Mathf.Clamp((hoursToday / dayLength) * 180, 10f, 170f);
                }
                else
                {
                    // During the the, rotate y back from 180 to 10 smootly. 
                    // This is only needed to rotate Sunflowers back to starting position
                    var nightProgress = (hoursToday - dayLength) / nightLength;
                    _y = Mathf.Clamp(180 - (nightProgress * 180), 10f, 170f);
                }
            }
            // Between Ticks, smoohtly transition between the rotation angles
            if (cappedTransitionPorgress < 1f)
            {
                __instance._sun.transform.localRotation = Quaternion.Lerp(_lastRotation, Quaternion.Euler(_x, _y, 0), cappedTransitionPorgress);
            }
            return false;
        }

    }

    /// <summary>
    /// Patch to show SunOptions on the In game Menu
    /// </summary>
    [HarmonyPatch(typeof(OptionsBox), "GetPanel")]
    public static class InGameMenuPanelPatch
    {
        public static void Postfix(ref VisualElement __result)
        {
            var loc = TimberAPI.DependencyContainer.GetInstance<ILoc>();
            VisualElement root = __result.Query("OptionsBox");
            Button button = new Button() { classList = { "menu-button" } };
            button.text = loc.T("menu.rotatingsun");
            button.clicked += SunMenu.OpenOptionsDelegate;
            root.Insert(6, button);
        }
    }

    /// <summary>
    /// Patch to show SunOptions on Main Menu
    /// </summary>
    [HarmonyPatch(typeof(MainMenuPanel), "GetPanel")]
    public static class MainMenuPanelPatch
    {
        public static void Postfix(ref VisualElement __result)
        {
            var loc = TimberAPI.DependencyContainer.GetInstance<ILoc>();
            VisualElement root = __result.Query("MainMenuPanel");
            Button button = new Button() { classList = { "menu-button" } };
            button.text = loc.T("menu.rotatingsun");
            button.clicked += SunMenu.OpenOptionsDelegate;
            root.Insert(6, button);
        }
    }

}
