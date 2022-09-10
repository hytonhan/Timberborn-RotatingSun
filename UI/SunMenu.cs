using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using TimberbornAPI.UIBuilderSystem;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.Length.Unit;

namespace SunFix.UI
{
    public class SunMenu : IPanelController
    {
        public static Action OpenOptionsDelegate;
        private readonly PanelStack _panelStack;
        private readonly UIBuilder _uiBuilder;
        private readonly ILoc _loc;

        private const int _sunAngleLow = 0;
        private const int _sunAngleHigh = 90;

        public const int _sunAngleMinDefault = 5;
        public const int _sunAngleMaxDefaultTemperate = 50;
        public const int _sunAngleMaxDefaultDrought = 80;

        public const int _moonAngleDefault = 50;

        private const string _rotatingOptionsHeaderLoc = "rotatingsun.optionsheader";
        private const string _rotatingEnabledLoc = "rotatingsun.enablesun";
        private const string _rotatingTemperateLowLoc = "rotatingsun.SunAngleLowTemperate";
        private const string _rotatingTemperateHighLoc = "rotatingsun.SunAngleHighTemperate";
        private const string _rotatingDroughtLowLoc = "rotatingsun.SunAngleLowDrought";
        private const string _rotatingDroughtHighLoc = "rotatingsun.SunAngleHighDrought";
        private const string _rotatingSunFlowerRotationEnabledLoc = "rotatingsun.enablerotatingsunflowers";
        private const string _rotatingMoonAngleLoc = "rotatingsun.MoonAngle";

        private Label _temperateLowLabel;
        private Label _temperateHighLabel;

        private Label _droughtLowLabel;
        private Label _droughtHighLabel;

        private Label _moongAngleLabel;

        public SunMenu(
            UIBuilder uiBuilder, 
            PanelStack panelStack,
            ILoc loc)
        {
            _uiBuilder = uiBuilder;
            _panelStack = panelStack;
            _loc = loc;
            OpenOptionsDelegate = OpenOptionsPanel;
        }

        private void OpenOptionsPanel()
        {
            _panelStack.HideAndPush(this);
        }

        /// <summary>
        /// Create the Options Panel
        /// </summary>
        /// <returns></returns>
        public VisualElement GetPanel()
        {
            UIBoxBuilder boxBuilder = _uiBuilder.CreateBoxBuilder()
                .SetHeight(new Length(520, Pixel))
                .SetWidth(new Length(600, Pixel))
                .ModifyScrollView(builder => builder.SetName("elementPreview"));

            var sunOptionsContent = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
            
            sunOptionsContent.AddPreset(factory => factory.Labels().DefaultHeader(_rotatingOptionsHeaderLoc, builder: builder => builder.SetStyle(style => { style.alignSelf = Align.Center; style.marginBottom = new Length(10, Pixel); })));
            sunOptionsContent.AddPreset(factory => factory.Toggles().Checkbox(locKey: _rotatingEnabledLoc, name: "EnableSunRotation", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));

            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.TemperateSunAngleLow) + "Label", text: $"{_loc.T(_rotatingTemperateLowLoc)}: {RotatingSunPlugin.TemperateSunAngleLow}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.TemperateSunAngleLow, name: nameof(RotatingSunPlugin.TemperateSunAngleLow), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.TemperateSunAngleHigh) + "Label", text: $"{_loc.T(_rotatingTemperateHighLoc)}: {RotatingSunPlugin.TemperateSunAngleHigh}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.TemperateSunAngleHigh, name: nameof(RotatingSunPlugin.TemperateSunAngleHigh), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));

            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.DroughtSunAngleLow) + "Label", text: $"{_loc.T(_rotatingDroughtLowLoc)}: {RotatingSunPlugin.DroughtSunAngleLow}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.DroughtSunAngleLow, name: nameof(RotatingSunPlugin.DroughtSunAngleLow), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.DroughtSunAngleHigh) + "Label", text: $"{_loc.T(_rotatingDroughtHighLoc)}: {RotatingSunPlugin.DroughtSunAngleHigh}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.DroughtSunAngleHigh, name: nameof(RotatingSunPlugin.DroughtSunAngleHigh), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            
            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.MoonAngle) + "Label", text: $"{_loc.T(_rotatingMoonAngleLoc)}: {RotatingSunPlugin.MoonAngle}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.MoonAngle, name: nameof(RotatingSunPlugin.MoonAngle), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));

            sunOptionsContent.AddPreset(factory => factory.Toggles().Checkbox(locKey: _rotatingSunFlowerRotationEnabledLoc, name: "EnableSunflowerRotation"));

            boxBuilder.AddComponent(sunOptionsContent.Build());

            VisualElement root = boxBuilder.AddCloseButton("CloseButton").SetBoxInCenter().AddHeader(_rotatingOptionsHeaderLoc).BuildAndInitialize();
            root.Q<Button>("CloseButton").clicked += OnUICancelled;
            root.Q<Toggle>("EnableSunRotation").RegisterValueChangedCallback(ToggleSunEnabled);
            root.Q<Toggle>("EnableSunRotation").value = RotatingSunPlugin.RotatingSunEnabled;
            root.Q<Toggle>("EnableSunflowerRotation").RegisterValueChangedCallback(ToggleSunflowerEnabled);
            root.Q<Toggle>("EnableSunflowerRotation").value = RotatingSunPlugin.RotatingSunFlowersEnabled;

            var temperateLowSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.TemperateSunAngleLow)).RegisterValueChangedCallback(TemperateSunAngleLowSliderChangedCallback);
            var temperateHighSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.TemperateSunAngleHigh)).RegisterValueChangedCallback(TemperateSunAngleHighSliderChangedCallback);

            _temperateLowLabel = root.Q<Label>(nameof(RotatingSunPlugin.TemperateSunAngleLow) + "Label");
            _temperateHighLabel = root.Q<Label>(nameof(RotatingSunPlugin.TemperateSunAngleHigh) + "Label");

            var droughtLowSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.DroughtSunAngleLow)).RegisterValueChangedCallback(DroughtSunAngleLowSliderChangedCallback);
            var droughtHighSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.DroughtSunAngleHigh)).RegisterValueChangedCallback(DroughtSunAngleHighSliderChangedCallback);

            _droughtLowLabel = root.Q<Label>(nameof(RotatingSunPlugin.DroughtSunAngleLow) + "Label");
            _droughtHighLabel = root.Q<Label>(nameof(RotatingSunPlugin.DroughtSunAngleHigh) + "Label");

            var moongAngleSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.MoonAngle)).RegisterValueChangedCallback(MoongAngleSliderChangedCallback);
            _moongAngleLabel = root.Q<Label>(nameof(RotatingSunPlugin.MoonAngle) + "Label");

            return root;
        }

        /// <summary>
        /// Save the value when toggled and patch/unpatch the rotatiing sun
        /// </summary>
        /// <param name="changeEvent"></param>
        private void ToggleSunEnabled(ChangeEvent<bool> changeEvent)
        {
            RotatingSunPlugin.RotatingSunEnabled = changeEvent.newValue;

            if(!RotatingSunPlugin.ConfigFile.TryGetEntry<bool>("General", nameof(RotatingSunPlugin.RotatingSunEnabled), out var setting))
            {
                RotatingSunPlugin.Log.LogInfo($"Config \"{nameof(RotatingSunPlugin.RotatingSunEnabled)}\" didn't exist. Creating.");
                RotatingSunPlugin.RotatingSunEnabled = 
                    RotatingSunPlugin.ConfigFile.Bind(
                        "General",
                        nameof(RotatingSunPlugin.RotatingSunEnabled),
                        true,
                        "Enable the Sun to rotate around the world instead of being tied to player camera.").Value;

            }
            setting.Value = changeEvent.newValue;
            RotatingSunPlugin.ConfigFile.Save();

            RotatingSunPlugin.PatchSunRotation();
        }

        /// <summary>
        /// Save value when toggled and disable custom sunflower class 
        /// </summary>
        /// <param name="changeEvent"></param>
        private void ToggleSunflowerEnabled(ChangeEvent<bool> changeEvent)
        {
            RotatingSunPlugin.RotatingSunFlowersEnabled = changeEvent.newValue;

            if(!RotatingSunPlugin.ConfigFile.TryGetEntry<bool>("General", nameof(RotatingSunPlugin.RotatingSunFlowersEnabled), out var setting))
            {
                RotatingSunPlugin.Log.LogInfo($"Config \"{nameof(RotatingSunPlugin.RotatingSunFlowersEnabled)}\" didn't exist. Creating.");
                RotatingSunPlugin.RotatingSunFlowersEnabled = 
                    RotatingSunPlugin.ConfigFile.Bind(
                        "General",
                        nameof(RotatingSunPlugin.RotatingSunFlowersEnabled),
                        false,
                        "Enable Sunflowers to rotate to face the Sun.").Value;

            }
            setting.Value = changeEvent.newValue;
            RotatingSunPlugin.ConfigFile.Save();

            RotatingSunPlugin.SetSunflowerRotation();
        }

        private void TemperateSunAngleLowSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.TemperateSunAngleLow = changeEvent.newValue;

            if (!RotatingSunPlugin.ConfigFile.TryGetEntry<int>("General", nameof(RotatingSunPlugin.TemperateSunAngleLow), out var setting))
            {
                RotatingSunPlugin.Log.LogInfo($"Config \"{nameof(RotatingSunPlugin.TemperateSunAngleLow)}\" didn't exist. Creating.");
                RotatingSunPlugin.TemperateSunAngleLow =
                    RotatingSunPlugin.ConfigFile.Bind(
                        "General",
                        nameof(RotatingSunPlugin.TemperateSunAngleLow),
                        _sunAngleMinDefault,
                        "Sun starting angle during Temperate weather.").Value;

            }
            setting.Value = changeEvent.newValue;
            RotatingSunPlugin.TemperateSunAngleLow = changeEvent.newValue;
            RotatingSunPlugin.ConfigFile.Save();

            Patches.XMinAngle = changeEvent.newValue;
            _temperateLowLabel.text = $"{_loc.T(_rotatingTemperateLowLoc)}: {changeEvent.newValue}";
        }

        private void TemperateSunAngleHighSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.TemperateSunAngleHigh = changeEvent.newValue;

            if (!RotatingSunPlugin.ConfigFile.TryGetEntry<int>("General", nameof(RotatingSunPlugin.TemperateSunAngleHigh), out var setting))
            {
                RotatingSunPlugin.Log.LogInfo($"Config \"{nameof(RotatingSunPlugin.TemperateSunAngleHigh)}\" didn't exist. Creating.");
                RotatingSunPlugin.TemperateSunAngleHigh =
                    RotatingSunPlugin.ConfigFile.Bind(
                        "General",
                        nameof(RotatingSunPlugin.TemperateSunAngleHigh),
                        _sunAngleMaxDefaultTemperate,
                        "Sun High angle during Temperate weather.").Value;

            }
            setting.Value = changeEvent.newValue;
            RotatingSunPlugin.ConfigFile.Save();

            Patches.XMaxAngle = changeEvent.newValue;
            RotatingSunPlugin.TemperateSunAngleHigh = changeEvent.newValue;
            _temperateHighLabel.text = $"{_loc.T(_rotatingTemperateHighLoc)}: {changeEvent.newValue}";
        }

        private void DroughtSunAngleLowSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.DroughtSunAngleLow = changeEvent.newValue;

            if (!RotatingSunPlugin.ConfigFile.TryGetEntry<int>("General", nameof(RotatingSunPlugin.DroughtSunAngleLow), out var setting))
            {
                RotatingSunPlugin.Log.LogInfo($"Config \"{nameof(RotatingSunPlugin.DroughtSunAngleLow)}\" didn't exist. Creating.");
                RotatingSunPlugin.DroughtSunAngleLow =
                    RotatingSunPlugin.ConfigFile.Bind(
                        "General",
                        nameof(RotatingSunPlugin.DroughtSunAngleLow),
                        _sunAngleMinDefault,
                        "Sun starting angle during Drought.").Value;

            }
            setting.Value = changeEvent.newValue;
            RotatingSunPlugin.DroughtSunAngleLow = changeEvent.newValue;
            RotatingSunPlugin.ConfigFile.Save();

            Patches.XDroughtMinAngle = changeEvent.newValue;
            _droughtLowLabel.text = $"{_loc.T(_rotatingDroughtLowLoc)}: {changeEvent.newValue}";
        }

        private void DroughtSunAngleHighSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.TemperateSunAngleHigh = changeEvent.newValue;

            if (!RotatingSunPlugin.ConfigFile.TryGetEntry<int>("General", nameof(RotatingSunPlugin.TemperateSunAngleHigh), out var setting))
            {
                RotatingSunPlugin.Log.LogInfo($"Config \"{nameof(RotatingSunPlugin.TemperateSunAngleHigh)}\" didn't exist. Creating.");
                RotatingSunPlugin.TemperateSunAngleHigh =
                    RotatingSunPlugin.ConfigFile.Bind(
                        "General",
                        nameof(RotatingSunPlugin.TemperateSunAngleHigh),
                        _sunAngleMaxDefaultTemperate,
                        "Sun High angle during Drought").Value;

            }
            setting.Value = changeEvent.newValue;
            RotatingSunPlugin.ConfigFile.Save();

            Patches.XDroughtMaxAngle = changeEvent.newValue;
            RotatingSunPlugin.TemperateSunAngleHigh = changeEvent.newValue;
            _droughtHighLabel.text = $"{_loc.T(_rotatingDroughtHighLoc)}: {changeEvent.newValue}";
        }

        private void MoongAngleSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.MoonAngle = changeEvent.newValue;

            if (!RotatingSunPlugin.ConfigFile.TryGetEntry<int>("General", nameof(RotatingSunPlugin.MoonAngle), out var setting))
            {
                RotatingSunPlugin.Log.LogInfo($"Config \"{nameof(RotatingSunPlugin.MoonAngle)}\" didn't exist. Creating.");
                RotatingSunPlugin.MoonAngle =
                    RotatingSunPlugin.ConfigFile.Bind(
                        "General",
                        nameof(RotatingSunPlugin.MoonAngle),
                        _moonAngleDefault,
                        "The Moon's angle.").Value;

            }
            setting.Value = changeEvent.newValue;
            RotatingSunPlugin.ConfigFile.Save();

            Patches.MoonAngle = changeEvent.newValue;
            RotatingSunPlugin.MoonAngle = changeEvent.newValue;
            _moongAngleLabel.text = $"{_loc.T(_rotatingMoonAngleLoc)}: {changeEvent.newValue}";
        }

        public bool OnUIConfirmed()
        {
            return false;
        }

        public void OnUICancelled()
        {
            _panelStack.Pop(this);
        }
    }
}
