using System;
using TimberApi.UiBuilderSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
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

            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.Config.TemperateSunAngleLow) + "Label", text: $"{_loc.T(_rotatingTemperateLowLoc)}: {RotatingSunPlugin.Config.TemperateSunAngleLow}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.Config.TemperateSunAngleLow, name: nameof(RotatingSunPlugin.Config.TemperateSunAngleLow), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.Config.TemperateSunAngleHigh) + "Label", text: $"{_loc.T(_rotatingTemperateHighLoc)}: {RotatingSunPlugin.Config.TemperateSunAngleHigh}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.Config.TemperateSunAngleHigh, name: nameof(RotatingSunPlugin.Config.TemperateSunAngleHigh), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));

            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.Config.DroughtSunAngleLow) + "Label", text: $"{_loc.T(_rotatingDroughtLowLoc)}: {RotatingSunPlugin.Config.DroughtSunAngleLow}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.Config.DroughtSunAngleLow, name: nameof(RotatingSunPlugin.Config.DroughtSunAngleLow), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.Config.DroughtSunAngleHigh) + "Label", text: $"{_loc.T(_rotatingDroughtHighLoc)}: {RotatingSunPlugin.Config.DroughtSunAngleHigh}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.Config.DroughtSunAngleHigh, name: nameof(RotatingSunPlugin.Config.DroughtSunAngleHigh), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            
            sunOptionsContent.AddPreset(factory => factory.Labels().GameTextBig(name: nameof(RotatingSunPlugin.Config.MoonAngle) + "Label", text: $"{_loc.T(_rotatingMoonAngleLoc)}: {RotatingSunPlugin.Config.MoonAngle}", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Sliders().SliderIntCircle(_sunAngleLow, _sunAngleHigh, value: RotatingSunPlugin.Config.MoonAngle, name: nameof(RotatingSunPlugin.Config.MoonAngle), builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));

            sunOptionsContent.AddPreset(factory => factory.Toggles().Checkbox(locKey: _rotatingSunFlowerRotationEnabledLoc, name: "EnableSunflowerRotation"));

            boxBuilder.AddComponent(sunOptionsContent.Build());

            VisualElement root = boxBuilder.AddCloseButton("CloseButton").SetBoxInCenter().AddHeader(_rotatingOptionsHeaderLoc).BuildAndInitialize();
            root.Q<Button>("CloseButton").clicked += OnUICancelled;
            root.Q<Toggle>("EnableSunRotation").RegisterValueChangedCallback(ToggleSunEnabled);
            root.Q<Toggle>("EnableSunRotation").value = RotatingSunPlugin.Config.RotatingSunEnabled;
            root.Q<Toggle>("EnableSunflowerRotation").RegisterValueChangedCallback(ToggleSunflowerEnabled);
            root.Q<Toggle>("EnableSunflowerRotation").value = RotatingSunPlugin.Config.RotatingSunFlowersEnabled;

            var temperateLowSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.Config.TemperateSunAngleLow)).RegisterValueChangedCallback(TemperateSunAngleLowSliderChangedCallback);
            var temperateHighSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.Config.TemperateSunAngleHigh)).RegisterValueChangedCallback(TemperateSunAngleHighSliderChangedCallback);

            _temperateLowLabel = root.Q<Label>(nameof(RotatingSunPlugin.Config.TemperateSunAngleLow) + "Label");
            _temperateHighLabel = root.Q<Label>(nameof(RotatingSunPlugin.Config.TemperateSunAngleHigh) + "Label");

            var droughtLowSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.Config.DroughtSunAngleLow)).RegisterValueChangedCallback(DroughtSunAngleLowSliderChangedCallback);
            var droughtHighSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.Config.DroughtSunAngleHigh)).RegisterValueChangedCallback(DroughtSunAngleHighSliderChangedCallback);

            _droughtLowLabel = root.Q<Label>(nameof(RotatingSunPlugin.Config.DroughtSunAngleLow) + "Label");
            _droughtHighLabel = root.Q<Label>(nameof(RotatingSunPlugin.Config.DroughtSunAngleHigh) + "Label");

            var moongAngleSlider = root.Q<SliderInt>(nameof(RotatingSunPlugin.Config.MoonAngle)).RegisterValueChangedCallback(MoongAngleSliderChangedCallback);
            _moongAngleLabel = root.Q<Label>(nameof(RotatingSunPlugin.Config.MoonAngle) + "Label");

            return root;
        }

        /// <summary>
        /// Save the value when toggled and patch/unpatch the rotatiing sun
        /// </summary>
        /// <param name="changeEvent"></param>
        private void ToggleSunEnabled(ChangeEvent<bool> changeEvent)
        {
            RotatingSunPlugin.Config.RotatingSunEnabled = changeEvent.newValue;
            RotatingSunPlugin.PatchSunRotation();
            RotatingSunPlugin.SaveConfig();
        }

        /// <summary>
        /// Save value when toggled and disable custom sunflower class 
        /// </summary>
        /// <param name="changeEvent"></param>
        private void ToggleSunflowerEnabled(ChangeEvent<bool> changeEvent)
        {
            RotatingSunPlugin.Config.RotatingSunFlowersEnabled = changeEvent.newValue;
            RotatingSunPlugin.SetSunflowerRotation();
            RotatingSunPlugin.SaveConfig();
        }

        private void TemperateSunAngleLowSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.Config.TemperateSunAngleLow = changeEvent.newValue;
            Patches.XMinAngle = changeEvent.newValue;
            _temperateLowLabel.text = $"{_loc.T(_rotatingTemperateLowLoc)}: {changeEvent.newValue}";
            RotatingSunPlugin.SaveConfig();
        }

        private void TemperateSunAngleHighSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.Config.TemperateSunAngleHigh = changeEvent.newValue;
            Patches.XMaxAngle = changeEvent.newValue;
            _temperateHighLabel.text = $"{_loc.T(_rotatingTemperateHighLoc)}: {changeEvent.newValue}";
            RotatingSunPlugin.SaveConfig();
        }

        private void DroughtSunAngleLowSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.Config.DroughtSunAngleLow = changeEvent.newValue;
            Patches.XDroughtMinAngle = changeEvent.newValue;
            _droughtLowLabel.text = $"{_loc.T(_rotatingDroughtLowLoc)}: {changeEvent.newValue}";
            RotatingSunPlugin.SaveConfig();
        }

        private void DroughtSunAngleHighSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.Config.TemperateSunAngleHigh = changeEvent.newValue;
            Patches.XDroughtMaxAngle = changeEvent.newValue;
            _droughtHighLabel.text = $"{_loc.T(_rotatingDroughtHighLoc)}: {changeEvent.newValue}";
            RotatingSunPlugin.SaveConfig();
        }

        private void MoongAngleSliderChangedCallback(ChangeEvent<int> changeEvent)
        {
            RotatingSunPlugin.Config.MoonAngle = changeEvent.newValue;
            Patches.MoonAngle = changeEvent.newValue;
            _moongAngleLabel.text = $"{_loc.T(_rotatingMoonAngleLoc)}: {changeEvent.newValue}";
            RotatingSunPlugin.SaveConfig();
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
