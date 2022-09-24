using HarmonyLib;
using SunFix.UI;
using System;
using System.Collections.Generic;
using TimberApi.DependencyContainerSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Localization;
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
        public static float XMinAngle = 5f;
        public static float XMaxAngle = 50f;
        public static float XDroughtMinAngle = 5f;
        public static float XDroughtMaxAngle = 80f;

        public static float MoonAngle = 50f;

        private static float _transitionProgress = 0f;
        private static float _transitionTime = 0.3f; // A Tick lasts 0.3 s
        private static float _lastTimestamp;
        private static float _x;
        private static float _y;
        private static Quaternion _lastRotation;

        private static float _dayLength = 0f;
        private static float _nightLength = 0f;

        [HarmonyPatch(typeof(Sun), nameof(Sun.RotateSunWithCamera))]
        public static bool Prefix(Sun __instance, DayStageTransition dayStageTransition)
        {
            InitDayLengths(__instance);
            var hoursToday = __instance._dayStageCycle._dayNightCycle.HoursPassedToday;
            var progress = (hoursToday / _dayLength);

            _transitionProgress += Time.deltaTime;
            float cappedTransitionPorgress = Math.Min(1f, _transitionProgress / _transitionTime);

            // Time in DayNightCycle is calculated in ticks. Calculate new rotation angles when 
            // a new tick is detected
            if (_lastTimestamp != hoursToday)
            {
                _transitionProgress = 0;
                _lastRotation = __instance._sun.transform.localRotation;
                _lastTimestamp = hoursToday;

                SetSunXAndYCoordinates(__instance, dayStageTransition, hoursToday, progress);
            }
            // Between Ticks, smoohtly transition between the rotation angles
            if (cappedTransitionPorgress < 1f)
            {
                __instance._sun.transform.localRotation = Quaternion.Lerp(_lastRotation, Quaternion.Euler(_x, _y, 0), cappedTransitionPorgress);
            }
            return false;
        }

        /// <summary>
        /// This calculates x and y angles of the Sun
        /// x is basically shadow's length.
        /// y is the sun's rotation.
        /// During day time, x goes 0 -> 90 -> 0 and y 0 -> 180, with little offsets
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="dayStageTransition"></param>
        /// <param name="hoursToday"></param>
        /// <param name="progress"></param>
        private static void SetSunXAndYCoordinates(Sun __instance, DayStageTransition dayStageTransition, float hoursToday, float progress)
        {
            if (progress <= 1)
            {
                if (progress <= 0.5)
                {
                    // in the morning sun goes up, maxing at _xOffset
                    if (__instance._dayStageCycle._weatherService._droughtService.IsDrought)
                    {
                        _x = Mathf.Clamp(progress * (XDroughtMaxAngle * 2) + XDroughtMinAngle, XDroughtMinAngle, XDroughtMaxAngle);
                    }
                    else
                    {
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
                _y = Mathf.Clamp((hoursToday / _dayLength) * 180, 10f, 170f);
            }
            else
            {
                var nightProgress = (hoursToday - _dayLength) / _nightLength;
                _y = Mathf.Clamp(180 - (nightProgress * 180), 10f, 170f);
                if (dayStageTransition.CurrentDayStage == DayStage.Night)
                {
                    float minAngle = __instance._dayStageCycle._weatherService._droughtService.IsDrought
                        ? XDroughtMinAngle
                        : XMinAngle;
                    
                    if (nightProgress <= 0.1)
                    {
                        _x = Mathf.Clamp(Mathf.Lerp(0f, 1f, nightProgress * 10) * (MoonAngle * 2) + minAngle, 0f, MoonAngle);
                    }
                    else if (nightProgress >= 0.9)
                    {
                        if(dayStageTransition.NextDayStageIsInDrought)
                        {
                            minAngle = XDroughtMinAngle;
                        }
                        _x = Mathf.Clamp(Mathf.Lerp(1f, 0f, (nightProgress - 0.9f) * 10) * (MoonAngle * 2) + minAngle, minAngle, MoonAngle);
                    }

                }
            }
        }

        private static void InitDayLengths(Sun __instance)
        {
            if (_dayLength == 0f)
            {
                _dayLength = __instance._dayStageCycle._dayNightCycle.DaytimeLengthInHours + 2;
            }
            if (_nightLength == 0f)
            {
                _nightLength = __instance._dayStageCycle._dayNightCycle.NighttimeLengthInHours - 2.5f;
            }
        }

        [HarmonyPatch(typeof(Sun), nameof(Sun.UpdateColors), new Type[] { typeof(DayStageTransition) })]
        [HarmonyPrefix]
        public static bool UpdateColorsPrefix(Sun __instance, DayStageTransition dayStageTransition)
        {
            DayStageColors dayStageColors = __instance.DayStageColors(dayStageTransition.CurrentDayStage);
            DayStageColors dayStageColors2 = __instance.DayStageColors(dayStageTransition.NextDayStage);
            float transitionProgress = dayStageTransition.TransitionProgress;
            __instance._sun.color = Color.Lerp(dayStageColors.SunColor, dayStageColors2.SunColor, transitionProgress);
            __instance._sun.intensity = Mathf.Lerp(dayStageColors.SunIntensity, dayStageColors2.SunIntensity, transitionProgress);
            //__instance._sun.shadowStrength = Mathf.Lerp(dayStageColors.ShadowStrength, dayStageColors2.ShadowStrength, transitionProgress);
            RenderSettings.ambientSkyColor = Color.Lerp(dayStageColors.AmbientSkyColor, dayStageColors2.AmbientSkyColor, transitionProgress);
            RenderSettings.ambientEquatorColor = Color.Lerp(dayStageColors.AmbientEquatorColor, dayStageColors2.AmbientEquatorColor, transitionProgress);
            RenderSettings.ambientGroundColor = Color.Lerp(dayStageColors.AmbientGroundColor, dayStageColors2.AmbientGroundColor, transitionProgress);
            RenderSettings.reflectionIntensity = Mathf.Lerp(dayStageColors.ReflectionsIntensity, dayStageColors2.ReflectionsIntensity, transitionProgress);
            FogSettings obj = (dayStageTransition.CurrentDayStageIsInDrought ? dayStageColors.DroughtFog : dayStageColors.TemperateWeatherFog);
            FogSettings fogSettings = (dayStageTransition.NextDayStageIsInDrought ? dayStageColors2.DroughtFog : dayStageColors2.TemperateWeatherFog);
            RenderSettings.fogColor = Color.Lerp(obj.FogColor, fogSettings.FogColor, transitionProgress);
            RenderSettings.fogDensity = Mathf.Lerp(obj.FogDensity, fogSettings.FogDensity, transitionProgress);


            InitDayLengths(__instance);
            var hoursToday = __instance._dayStageCycle._dayNightCycle.HoursPassedToday;
            var nightProgress = (hoursToday - _dayLength) / _nightLength;
            
            SetShadowStrengthDuringNightAndSunrise(__instance, dayStageTransition, dayStageColors, dayStageColors2, transitionProgress, nightProgress);

            return false;
        }

        private static void SetShadowStrengthDuringNightAndSunrise(Sun __instance, DayStageTransition dayStageTransition, DayStageColors dayStageColors, DayStageColors dayStageColors2, float transitionProgress, float nightProgress)
        {
            if (dayStageTransition.CurrentDayStage == DayStage.Night)
            {
                if (nightProgress <= 0.15f)
                {
                    __instance._sun.shadowStrength = Mathf.Lerp(0f, 0.8f, (nightProgress - 0.05f) * 10);
                }
                else if (nightProgress >= 0.85f)
                {
                    __instance._sun.shadowStrength = Mathf.Lerp(0.8f, 0f, (nightProgress - 0.85f) * 10);
                }
            }
            else
            {
                if (dayStageTransition.CurrentDayStage == DayStage.Sunrise)
                {
                    __instance._sun.shadowStrength = Mathf.Lerp(0f, 1f, dayStageTransition.TransitionProgress);
                }
                else
                {
                    __instance._sun.shadowStrength = Mathf.Lerp(dayStageColors.ShadowStrength, dayStageColors2.ShadowStrength, transitionProgress);
                }
            }
        }
    }

    /// <summary>
    /// Patch to show SunOptions on the In game Menu
    /// </summary>
    [HarmonyPatch(typeof(OptionsBox), "GetPanel")]
    public class InGameMenuPanelPatch
    {
        public static void Postfix(ref VisualElement __result)
        {
            var loc = DependencyContainer.GetInstance<ILoc>();
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
            var loc = DependencyContainer.GetInstance<ILoc>();
            VisualElement root = __result.Query("MainMenuPanel");
            Button button = new Button() { classList = { "menu-button" } };
            button.text = loc.T("menu.rotatingsun");
            button.clicked += SunMenu.OpenOptionsDelegate;
            root.Insert(6, button);
        }
    }
}
