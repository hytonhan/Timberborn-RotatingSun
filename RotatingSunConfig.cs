using System;
using System.Collections.Generic;
using System.Text;
using TimberApi.ConfigSystem;

namespace SunFix
{
    public class RotatingSunConfig : IConfig
    {
        public string ConfigFileName => "RotatingSun";

        public bool RotatingSunEnabled = true;
        public bool RotatingSunFlowersEnabled = false;

        public int TemperateSunAngleLow = 5;
        public int TemperateSunAngleHigh = 50;
        public int DroughtSunAngleLow = 5;
        public int DroughtSunAngleHigh = 80;
        public int MoonAngle = 50;
    }
}
