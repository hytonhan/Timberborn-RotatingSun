using HarmonyLib;
using SunFix.UI;
using System;
using Timberborn.MainMenuScene;
using Timberborn.Options;
using Timberborn.SkySystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace SunFix
{

    [HarmonyPatch]
    public static class Patches
    {
        private static readonly float _xOffset = 50f;
        private static float _transitionProgress = 0f;
        private static float _transitionTime = 0.3f; // A Tick lasts 0.3 s
        private static float _lastTimestamp;
        private static float _x;
        private static float _y;
        private static Quaternion _lastRotation;

        [HarmonyPatch(typeof(Sun), nameof(Sun.RotateSunWithCamera))]
        public static bool Prefix(Sun __instance)
        {
            var hoursToday = __instance._dayNightCycle.HoursPassedToday;
            var dayLength = __instance._dayNightCycle.DaytimeLengthInHours;
            var nightLength = __instance._dayNightCycle.NighttimeLengthInHours;

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
                        _x = Mathf.Clamp(progress * (_xOffset * 2) + 5f, 5f, _xOffset);
                    }
                    else
                    {
                        // in the afternoon sun goes down
                        _x = Mathf.Clamp((_xOffset * 2) - ((progress) * (_xOffset * 2)), 0f, _xOffset);
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
            VisualElement root = __result.Query("OptionsBox");
            Button button = new Button() { classList = { "menu-button" } };
            button.text = "Rotating Sun Options";
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
            VisualElement root = __result.Query("MainMenuPanel");
            Button button = new Button() { classList = { "menu-button" } };
            button.text = "Rotating Sun Options";
            button.clicked += SunMenu.OpenOptionsDelegate;
            root.Insert(6, button);
        }
    }

}
