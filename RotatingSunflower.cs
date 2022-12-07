using Bindito.Core;
using System;
using Timberborn.SkySystem;
using UnityEngine;

namespace SunFix
{
    public class RotatingSunflower : MonoBehaviour
    {
        private Sun _sun;

        private float _initialXPos;
        private float _initialYPos;
        private float _initialZPos;

        private float _r;

        [Inject]
        public void InjectDependencies(Sun sun)
        {
            _sun = sun;
        }

        public void Awake()
        {
            if (this.name.Contains("Sun"))
            {

                enabled = RotatingSunPlugin.Config.RotatingSunFlowersEnabled;
            }
            else
            {
                enabled = false;
            }
        }

        public void Start()
        {
            _r = (float)Math.Sqrt(Math.Pow(0.5, 2) + Math.Pow(0.5, 2));
            _initialXPos = gameObject.transform.localPosition.x - 0.5f + (_r * Mathf.Sin((gameObject.transform.localEulerAngles.y + 45) * Mathf.Deg2Rad));
            _initialYPos = gameObject.transform.localPosition.y;
            _initialZPos = gameObject.transform.localPosition.z - 0.5f + (_r * Mathf.Cos((gameObject.transform.localEulerAngles.y + 45) * Mathf.Deg2Rad));
        }

        /// <summary>
        /// When Sunflowers get disabled, reset their position and rotation
        /// </summary>
        public void OnDisable()
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.localPosition = new Vector3(_initialXPos, _initialYPos, _initialZPos);
        }

        /// <summary>
        /// Rotate the entity based on the Sun's y-angle. Also fix the position caused by rotation.
        /// </summary>
        public void Update()
        {
            if (enabled)
            {
                gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localEulerAngles.x,
                                                                      _sun.transform.localEulerAngles.y,
                                                                      gameObject.transform.localEulerAngles.z);

                gameObject.transform.localPosition = new Vector3(_initialXPos + 0.5f - (_r * Mathf.Sin((_sun.transform.localEulerAngles.y + 45) * Mathf.Deg2Rad)),
                                                                 _initialYPos,
                                                                 _initialZPos + 0.5f - (_r * Mathf.Cos((_sun.transform.localEulerAngles.y + 45) * Mathf.Deg2Rad)));
            }
        }
    }
}
