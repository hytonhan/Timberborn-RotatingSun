// using HarmonyLib;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Reflection.Emit;
// using TimberApi.DependencyContainerSystem;
// using Timberborn.HazardousWeatherSystem;
// using Timberborn.SkySystem;
// using Timberborn.TimeSystem;
// using Timberborn.WeatherSystem;
// using UnityEngine;

// namespace SunFix
// {
//     [HarmonyPatch]
//     public class Patches
//     {

//         public static bool SunEnabled = true;
//         public static float XMinAngle = 5f;
//         public static float XMaxAngle = 50f;
//         public static float XDroughtMinAngle = 5f;
//         public static float XDroughtMaxAngle = 80f;
//         public static float XBadtideMinAngle = 5f;
//         public static float XBadtideMaxAngle = 60f;

//         public static float MoonAngle = 50f;

//         private static float _transitionProgress = 0f;
//         private static float _lastTimestamp = 0;
//         private static float _x;
//         private static float _y;
//         private static Quaternion _lastRotation;

//         private static  DroughtWeather _droughtWeather;
//         private static BadtideWeather _badtideWeather;

//         private static RotatingSunConfig _config = null;

//         private static float _dayLength = 0f;
//         private static float _nightLength = 0f;


//         private static Light _light;
//         private static WeatherService _weatherService;
//         private static HazardousWeatherService _hazardousWeatherService;

//         [HarmonyPatch(typeof(Sun), "RotateSunWithCamera")]
//         public static bool Prefix(Sun __instance, DayStageTransition dayStageTransition)
//         {
//             if (_droughtWeather == null) {
//                 _droughtWeather = DependencyContainer.GetInstance<DroughtWeather>();
//             }
//             if (_badtideWeather == null) {
//                 _badtideWeather = DependencyContainer.GetInstance<BadtideWeather>();
//             }
//             if (_config == null) {
//                 _config = DependencyContainer.GetInstance<RotatingSunConfig>();
//                 SunEnabled = _config.RotatingSunEnabled.Value;
//                 XMinAngle = _config.TemperateSunAngleLow.Value;
//                 XMaxAngle = _config.TemperateSunAngleHigh.Value;
//                 XDroughtMinAngle = _config.DroughtSunAngleLow.Value;
//                 XDroughtMaxAngle = _config.DroughtSunAngleHigh.Value;
//                 XBadtideMinAngle = _config.BadtideSunAngleLow.Value;
//                 XBadtideMaxAngle = _config.BadtideSunAngleHigh.Value;
//                 MoonAngle = _config.MoonAngle.Value;
//             }
//             if (!SunEnabled) {
//                 return true;
//             }
//             _transitionProgress += Time.deltaTime;
//             var dayStageCycle = (DayStageCycle)typeof(Sun).GetField("_dayStageCycle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            
//             var dayNightCycle = (IDayNightCycle)typeof(DayStageCycle).GetField("_dayNightCycle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dayStageCycle);

//             if (_lastTimestamp != dayNightCycle.HoursPassedToday)
//             {
//                 _lastTimestamp = dayNightCycle.HoursPassedToday;
//                 _transitionProgress = 0;
//             }

//             InitDayLengths(dayNightCycle);
//             var progress = (_lastTimestamp + _transitionProgress / dayNightCycle.ConfiguredDayLengthInSeconds * 24f) / _dayLength;
//             if (_light == null) {
//                 _light = (Light)typeof(Sun).GetField("_sun", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
//             }
//             _lastRotation = _light.transform.localRotation;

//             if(_weatherService == null) {
//                 _weatherService = (WeatherService)typeof(DayStageCycle).GetField("_weatherService",  BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dayStageCycle);
//             }
//             if (_hazardousWeatherService == null) {
//                 _hazardousWeatherService = (HazardousWeatherService)typeof(DayStageCycle).GetField("_hazardousWeatherService",  BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dayStageCycle);
//             }
//             /// if in map-editor, this is null
//             if (_hazardousWeatherService.CurrentCycleHazardousWeather == null) {
//                 return true;
//             }

//             if (progress <= 1)
//             {
//                 if (_weatherService.IsHazardousWeather)
//                 {
//                     if (_hazardousWeatherService.CurrentCycleHazardousWeather.GetType() == typeof(DroughtWeather))
//                     {
//                         if (progress <= 0.5)
//                         {
//                             var tempProgress = progress * 2;
//                             _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(XDroughtMinAngle, 10f, 0f), Quaternion.Euler(XDroughtMaxAngle, 85f, 0), tempProgress);
//                         }
//                         else
//                         {
//                             var tempProgress = (progress - 0.5f) * 2f;
//                             _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(XDroughtMaxAngle, 85f, 0f), Quaternion.Euler(XDroughtMinAngle, 170f, 0), tempProgress);
//                         }
//                     }
//                     else if (_hazardousWeatherService.CurrentCycleHazardousWeather.GetType() == typeof(BadtideWeather))
//                     {
//                         if (progress <= 0.5)
//                         {
//                             var tempProgress = progress * 2;
//                             _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(XBadtideMinAngle, 10f, 0f), Quaternion.Euler(XBadtideMaxAngle, 85f, 0), tempProgress);
//                         }
//                         else
//                         {
//                             var tempProgress = (progress - 0.5f) * 2f;
//                             _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(XBadtideMaxAngle, 85f, 0f), Quaternion.Euler(XBadtideMinAngle, 170f, 0), tempProgress);
//                         }

//                     }
//                 }
//                 else
//                 {
//                     if (progress <= 0.5)
//                     {
//                         var tempProgress = progress * 2;
//                         _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(XMinAngle, 10f, 0f), Quaternion.Euler(XMaxAngle, 85f, 0), tempProgress);
//                     }
//                     else
//                     {
//                         var tempProgress = (progress - 0.5f) * 2f;
//                         _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(XMaxAngle, 85f, 0f), Quaternion.Euler(XMinAngle, 170f, 0), tempProgress);
//                     }
//                 }
//             }
//             else
//             {
//                 var nightProgress = (_lastTimestamp + ((_transitionProgress /  dayNightCycle.ConfiguredDayLengthInSeconds * 24f) - _dayLength)) / _nightLength;
//                 _y = Mathf.Clamp(180 - (nightProgress * 180), 10f, 170f);
//                 if (dayStageTransition.CurrentDayStage == DayStage.Night)
//                 {
//                     float minAngle = _weatherService.IsHazardousWeather
//                         ? _hazardousWeatherService.CurrentCycleHazardousWeather.GetType() == typeof(DroughtWeather)
//                             ? XDroughtMinAngle
//                             : XBadtideMinAngle
//                         : XMinAngle;

//                     if (nightProgress <= 0.1)
//                     {
//                         _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(minAngle, 170f, 0f),
//                                                                                   Quaternion.Euler(MoonAngle, 170f, 0),
//                                                                                   nightProgress * 10);
//                     }
//                     else if (nightProgress >= 0.80)
//                     {
//                         if (dayStageTransition.NextDayStageHazardousWeatherId == _droughtWeather.Id)
//                         {
//                             minAngle = XDroughtMinAngle;
//                         }
//                         else if (dayStageTransition.NextDayStageHazardousWeatherId == _badtideWeather.Id)
//                         {
//                             minAngle = XBadtideMinAngle;
//                         }
//                         _x = Mathf.Clamp(Mathf.Lerp(1f, 0f, (nightProgress - 0.9f) * 10) * (MoonAngle * 2) + minAngle, minAngle, MoonAngle);
//                         _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(MoonAngle, 10f, 0f),
//                                                                         Quaternion.Euler(minAngle, 10f, 0),
//                                                                         (nightProgress - 0.80f) * 10);
//                     }
//                     else
//                     {
//                         _light.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(MoonAngle, 170f, 0f),
//                                                                         Quaternion.Euler(MoonAngle, 10f, 0),
//                                                                         (nightProgress - 0.1f) * 1.25f);
//                     }

//                 }
//             }
//             return false;
//         }


//         private static void InitDayLengths(IDayNightCycle dayNightCycle)
//         {
//             if (_dayLength == 0f)
//             {
//                 _dayLength = dayNightCycle.DaytimeLengthInHours + 2;
//             }
//             if (_nightLength == 0f)
//             {
//                 _nightLength = dayNightCycle.NighttimeLengthInHours - 2f;
//             }
//         }


//         [HarmonyPatch(typeof(Sun), "UpdateColors", new Type[] { typeof(DayStageTransition) })]
//         static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original) 
//         {
//             var shadowMethod = typeof(Patches).GetMethod("SetShadowStrengthDuringNightAndSunrise", BindingFlags.Static | BindingFlags.NonPublic);
//             var intensitySetter= typeof(Light).GetProperty("intensity", BindingFlags.Public | BindingFlags.Instance)
//                                               .GetAccessors()[0];

//             var getCurrentDayStage = typeof(DayStageTransition).GetProperty("CurrentDayStage", BindingFlags.Public | BindingFlags.Instance)
//                                                                .GetAccessors()[0];
//             var getCurrentTransitionProgress = typeof(DayStageTransition).GetProperty("TransitionProgress", BindingFlags.Public | BindingFlags.Instance)
//                                                                          .GetAccessors()[0];
//             var found = false;

//             var codes = new List<CodeInstruction>(instructions);
//             for (int i = 0; i < codes.Count; i++)
//             {
//                 if(codes[i].opcode != OpCodes.Callvirt) {
//                     continue;
//                 }

//                 if (codes[i].opcode == OpCodes.Callvirt && (MethodInfo)codes[i].operand == intensitySetter)
//                 {
//                     codes.RemoveRange(i+3, 6);
//                     var newCodes = new List<CodeInstruction>();
//                     var dayStageCycle = typeof(Sun).GetField("_dayStageCycle", BindingFlags.Instance | BindingFlags.NonPublic);
//                     var dayNightCycle = typeof(DayStageCycle).GetField("_dayNightCycle", BindingFlags.Instance | BindingFlags.NonPublic);
//                     var hoursPassedToday = typeof(IDayNightCycle).GetProperty("HoursPassedToday").GetAccessors()[0];
                    
//                     newCodes.Add(new CodeInstruction(OpCodes.Ldarga_S, 1));
//                     newCodes.Add(new CodeInstruction(OpCodes.Callvirt, getCurrentDayStage));

//                     newCodes.Add(new CodeInstruction(OpCodes.Ldarga_S, 1));
//                     newCodes.Add(new CodeInstruction(OpCodes.Callvirt, getCurrentTransitionProgress));

//                     newCodes.Add(new CodeInstruction(OpCodes.Ldarg_0));
//                     newCodes.Add(new CodeInstruction(OpCodes.Ldfld, dayStageCycle));
//                     newCodes.Add(new CodeInstruction(OpCodes.Ldfld, dayNightCycle));
//                     newCodes.Add(new CodeInstruction(OpCodes.Callvirt, hoursPassedToday));

//                     newCodes.Add(new CodeInstruction(OpCodes.Call, shadowMethod));

//                     codes.InsertRange(i+3, newCodes);

//                     found = true;
//                     break;
//                 }
//             }
//             if (found is false) {
//                 UnityEngine.Debug.Log($"intensity setter not found");
//             }
            
//             return codes.AsEnumerable();
//         }

//         private static float SetShadowStrengthDuringNightAndSunrise(
//             DayStage currentDayStage,
//             float transitionProgress,
//             float hoursToday)
//         {
//             var nightProgress = (hoursToday - _dayLength) / _nightLength;
//             if (currentDayStage == DayStage.Night)
//             {
//                 if (nightProgress <= 0.15f)
//                 {
//                     return Mathf.Lerp(0f, 0.8f, (nightProgress - 0.05f) * 10);
//                 }
//                 else if( nightProgress >= 0.9f)
//                 {
//                     return Mathf.Lerp(0f, 0.5f, (nightProgress - 0.9f) * 10);
//                 }
//                 else if (nightProgress >= 0.75f)
//                 {
//                     return Mathf.Lerp(0.8f, 0f, (nightProgress - 0.75f)  * 20);
//                 }
//                 else {
//                     return 0.8f;
//                 }
//             }
//             else if (currentDayStage == DayStage.Sunrise)
//             {
//                 return Mathf.Lerp(0.5f, 1f, transitionProgress);
//             }
//             else if (currentDayStage == DayStage.Sunset)
//             {
//                 return Mathf.Lerp(1f, 0f, transitionProgress);
//             }
//             else
//             {
//                 return 1f;
//             }
//         }
//     }
// }
