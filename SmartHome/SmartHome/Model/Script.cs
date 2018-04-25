using System.Collections.Generic;
using Java.Util;
using Android.Graphics;

namespace SmartHome
{
    public class Script
    {
        public List<Device> Devices { get; set; }

        public int color { get; set; }

        public string name { get; set; }

        public bool isActive { get; set; }

        public Script() { Devices = new List<Device>(); }
        public Script(string n, int col) {
            name = n;
            color = col;
            Devices = new List<Device>();
        }
    }
}