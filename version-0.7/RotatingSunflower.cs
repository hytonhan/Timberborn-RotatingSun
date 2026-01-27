// using Bindito.Core;
// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using Timberborn.BaseComponentSystem;
// using Timberborn.Buildings;
// using Timberborn.SkySystem;
// using Timberborn.TickSystem;
// using Timberborn.Timbermesh;
// using Timberborn.TimbermeshDTO;
// using Timberborn.TransformControl;
// using UnityEngine;
//
// namespace SunFix
// {
//     public class RotatingSunflower : BaseComponent, IAwakableComponent, IStartableComponent, IUpdatableComponent
//     {
//         private Sun _sun;
//         private RotatingSunConfig _config;
//
//         private float _initialXPos;
//         private float _initialYPos;
//         private float _initialZPos;
//
//         private float _r;
//
//         [Inject]
//         public void InjectDependencies(
//             Sun sun,
//             RotatingSunConfig config)
//         {
//             _sun = sun;
//             _config = config;
//         }
//
//         public void Awake()
//         {
//             if (this.Name.Contains("Sun") && _config.RotatingSunFlowersEnabled.Value == true)
//             {
//                 this.EnableComponent();
//             }
//             else
//             {
//                 this.DisableComponent();
//             }
//         }
//
//         public void Start()
//         {
//             _r = (float)Math.Sqrt(Math.Pow(0.5, 2) + Math.Pow(0.5, 2));
//             _initialXPos = GameObject.transform.localPosition.x - 0.5f +
//                            (_r * Mathf.Sin((GameObject.transform.localEulerAngles.y + 45) * Mathf.Deg2Rad));
//             _initialYPos = GameObject.transform.localPosition.y;
//             _initialZPos = GameObject.transform.localPosition.z - 0.5f +
//                            (_r * Mathf.Cos((GameObject.transform.localEulerAngles.y + 45) * Mathf.Deg2Rad));
//
//
//             foreach (var component in this.AllComponents)
//             {
//                 Debug.Log(component.GetType().Name);
//             }
//
//             var transformController = GetComponent<TransformController>();
//             var rotationModifiers = (SortedList<int, RotationModifier>)typeof(TransformController).GetField(
//                 "_rotationModifiers", 
//                 BindingFlags.NonPublic | BindingFlags.Instance)
//                 .GetValue(transformController);
//
//             foreach (var modifier in rotationModifiers)
//             {
//                 Debug.Log($"Rotation: {modifier.Key} - {modifier.Value} - {modifier.Value.Value.eulerAngles.x} - {modifier.Value.Value.eulerAngles.y} - {modifier.Value.Value.eulerAngles.z}");
//             }
//
//             var anim = GameObject.GetComponentInChildren<Timberborn.TimbermeshAnimations.IAnimator>();
//             Debug.Log(anim);
//             var animCont = GameObject.GetComponentInChildren<Timberborn.TimbermeshAnimations.IAnimatorController>();
//             Debug.Log(animCont);
//             
//             var nodeAnim2 = GetComponentInChildren<TimbermeshAnimator>();
//             if (nodeAnim2 == null) Debug.Log($"node is null");
//             
//             var nodeAnim = GetComponentInChildren<NodeAnimation>();
//             if (nodeAnim == null) Debug.Log($"node is null");
//
//         }
//
//         public void OnDisable()
//         {
//             GameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
//             GameObject.transform.localPosition = new Vector3(_initialXPos, _initialYPos, _initialZPos);
//         }
//
//         public void Update()
//         {
//             if (!this.Enabled)
//             {
//                 return;
//             }
//             // var vertexAnim = GetComponentInChildren<VertexAnimation>();
//             // if (vertexAnim == null) Debug.Log($"vertex is null");
//             // var nodeAnim = GetComponentInChildren<NodeAnimation>();
//             // if (nodeAnim == null) Debug.Log($"node is null");
//             
//             
//             // Debug.Log($"Before: x: {GameObject.transform.localEulerAngles.x} \n\ty: {GameObject.transform.localEulerAngles.y}\n\tz: {GameObject.transform.localEulerAngles.z}");
//
//             // Debug.Log("test");
//
//             // Transform.TransformDirection(0, _sun.Transform.rotation.y, 0);
//
//             // SUN day y 10 -> 170, night 170 -> 10
//             // flower. 
//             Transform.Rotate(0, 0, 0);
//             // Debug.Log($"Sun y: {_sun.Transform.localEulerAngles.y} | flower y: {Transform.localEulerAngles.y}");
//             var diff = _sun.Transform.localEulerAngles.y - Transform.localEulerAngles.y;
//             Transform.Rotate(new Vector3(0, diff, 0));
//             
//             // if (GameObject.transform.localEulerAngles.y != _sun.Transform.localEulerAngles.y)
//             // {
//             //     Transform.Dire
//             //     Transform.LookAt(_sun.Transform);
//             //     // Transform.Rotate(0, _sun.Transform.localRotation.y,
//             //     // 0);
//             //     // GameObject.transform.localRotation = Quaternion.Euler(GameObject.transform.localEulerAngles.x,
//             //     //     _sun.Transform.localEulerAngles.y,
//             //     //     GameObject.transform.localEulerAngles.z);
//             //     //Debug.Log($"After: x: {GameObject.transform.localEulerAngles.x} \n\ty: {GameObject.transform.localEulerAngles.y}\n\tz: {GameObject.transform.localEulerAngles.z}");
//             //
//             // }
//             // GameObject.transform.localPosition = new Vector3(
//             //     _initialXPos + 0.5f - (_r * Mathf.Sin((_sun.Transform.localEulerAngles.y + 45) * Mathf.Deg2Rad)),
//             //     _initialYPos,
//             //     _initialZPos + 0.5f - (_r * Mathf.Cos((_sun.Transform.localEulerAngles.y + 45) * Mathf.Deg2Rad)));
//
//         }
//     }
// }
