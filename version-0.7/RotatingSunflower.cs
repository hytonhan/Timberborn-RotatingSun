using Bindito.Core;
using System;
using Timberborn.SkySystem;
using UnityEngine;

namespace SunFix
{
    public class RotatingSunflower : MonoBehaviour
    {
        private Sun _sun;
        private RotatingSunConfig _config;

        private float _initialXPos;
        private float _initialYPos;
        private float _initialZPos;

        private float _r;

        [Inject]
        public void InjectDependencies(
            Sun sun,
            RotatingSunConfig config)
        {
            _sun = sun;
            _config = config;
        }

        public void Awake()
        {
            if (this.name.Contains("Sun"))
            {

                enabled = _config.RotatingSunFlowersEnabled.Value;
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

        public void OnDisable()
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.localPosition = new Vector3(_initialXPos, _initialYPos, _initialZPos);
        }

        public void Update()
        {
            if (enabled)
            {
                gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localEulerAngles.x,
                                                                      _sun.Transform.localEulerAngles.y,
                                                                      gameObject.transform.localEulerAngles.z);

                gameObject.transform.localPosition = new Vector3(_initialXPos + 0.5f - (_r * Mathf.Sin((_sun.Transform.localEulerAngles.y + 45) * Mathf.Deg2Rad)),
                                                                 _initialYPos,
                                                                 _initialZPos + 0.5f - (_r * Mathf.Cos((_sun.Transform.localEulerAngles.y + 45) * Mathf.Deg2Rad)));
            }
        }
    }
}
