using HarmonyLib;
using System;
using Timberborn.CoreUI;
using Timberborn.MainMenuScene;
using Timberborn.Options;
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

        public SunMenu(
            UIBuilder uiBuilder, 
            PanelStack panelStack)
        {
            _uiBuilder = uiBuilder;
            _panelStack = panelStack;
            OpenOptionsDelegate = OpenOptionsPanel;
        }

        private void OpenOptionsPanel()
        {
            Console.WriteLine("foo2");
            _panelStack.HideAndPush(this);
        }

        /// <summary>
        /// Create the Options Panel
        /// </summary>
        /// <returns></returns>
        public VisualElement GetPanel()
        {

            Console.WriteLine("foo");
            UIBoxBuilder boxBuilder = _uiBuilder.CreateBoxBuilder()
                .SetHeight(new Length(200, Pixel))
                .SetWidth(new Length(600, Pixel))
                .ModifyScrollView(builder => builder.SetName("elementPreview"));

            var sunOptionsContent = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
            sunOptionsContent.AddPreset(factory => factory.Labels().DefaultHeader("rotatingsun.optionsheader", builder: builder => builder.SetStyle(style => { style.alignSelf = Align.Center; style.marginBottom = new Length(10, Pixel); })));
            sunOptionsContent.AddPreset(factory => factory.Toggles().Checkbox(locKey: "rotatingsun.enablesun", name: "EnableSunRotation", builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            sunOptionsContent.AddPreset(factory => factory.Toggles().Checkbox(locKey: "rotatingsun.enablerotatingsunflowers", name: "EnableSunflowerRotation"));

            boxBuilder.AddComponent(sunOptionsContent.Build());

            VisualElement root = boxBuilder.AddCloseButton("CloseButton").SetBoxInCenter().AddHeader("rotatingsun.optionsheader").BuildAndInitialize();
            root.Q<Button>("CloseButton").clicked += OnUICancelled;
            root.Q<Toggle>("EnableSunRotation").RegisterValueChangedCallback(ToggleSunEnabled);
            root.Q<Toggle>("EnableSunRotation").value = RotatingSunPlugin.RotatingSunEnabled;
            root.Q<Toggle>("EnableSunflowerRotation").RegisterValueChangedCallback(ToggleSunflowerEnabled);
            root.Q<Toggle>("EnableSunflowerRotation").value = RotatingSunPlugin.RotatingSunFlowersEnabled;

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
            Console.WriteLine("Sunflowers toggled");
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
