using System;
using UnityEngine;

namespace StupidTemplate.Classes
{
    public class ExtGradient
    {
        public GradientColorKey[] colors = new GradientColorKey[]
        {
            new GradientColorKey(new Color32(0x06, 0x06, 0x06, 0xFF), 1f),
        };

        public bool isRainbow = false;
        public bool copyRigColors = false;
    }
}
