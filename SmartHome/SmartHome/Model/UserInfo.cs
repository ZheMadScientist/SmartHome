using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SmartHome.Model
{
    public class UserInfo
    {

        public DeviceGroups group { get; set; }
        public List<Script> scripts { get; set; }

        public UserInfo()
        {
            // Default constructor required for calls to DataSnapshot.getValue(UserInfo.class)

            group = new DeviceGroups(FireBall.devicesRequested.ToArray());
            scripts = FireBall.scripts;
        }

        public UserInfo(Device[] d, List<Script> Scripts)
        {
            group = new DeviceGroups(d);
            scripts = Scripts;
        }

        public void ChangeDevices(int mode, Device device) {
            // 0 - delete, 1 - add, 2 - change
            if (mode == 0) {
                //devices.Remove(device);
                group.mDevices.Remove(device);
            }

            if (mode == 1) {
                //devices.Add(device);
                group.mDevices.Add(device);
            }

            if (mode == 2) {
                for (int i = 0; i < group.mDevices.Count; i++) {
                    if (group.mDevices.ElementAt(i).DeviceName.Equals(device.DeviceName)) {
                        //devices[i] = device;
                        group.mDevices[i] = device;
                    }
                }
                
            }
        }


    }
}