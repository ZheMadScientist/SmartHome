using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome
{
    public class Device : IEquatable<Device>
    {


        private string pDeviceName;
        private string pDeviceDescription;
        private string pDeviceGroup;
        private bool pCondition;
        private int pExactCondition = Constants.Constants.didntused;
        private bool pisFavor;

        public Device() { }
        public Device(string name, string description, string group, bool condition, bool isExactConditionNeeded, int exactCondition, bool isFavor)
        {
            pDeviceName = name;
            pDeviceDescription = description;
            pDeviceGroup = group;

            if (isExactConditionNeeded)
            {
                pExactCondition = exactCondition;
            }
            pisFavor = isFavor;

        }


        public string DeviceName
        {
            set { pDeviceName = value; }
            get { return pDeviceName; }
        }
        public string DeviceDescription
        {
            set { pDeviceDescription = value; }
            get { return pDeviceDescription; }
        }
        public string DeviceGroup
        {
            set { pDeviceGroup = value; }
            get { return pDeviceGroup; }
        }
        public bool Condition
        {
            set { pCondition = value; }
            get { return pCondition; }
        }
        public int ExCondition {
            set { pExactCondition = value; }
            get { return pExactCondition; }
        }
        public bool Favor
        {
            set { pisFavor = value; }
            get { return pisFavor; }
        }

        public bool Equals(Device other)
        {
            return DeviceName.Equals(other.DeviceName) && DeviceDescription.Equals(other.DeviceDescription) && DeviceGroup.Equals(other.DeviceGroup); 
        }
    }


    public class DeviceGroups
    {
        public List<Device> mDevices { get; set; }

        public DeviceGroups() { }

        public DeviceGroups(Device[] acarrofdevices)
        {
            mDevices = new List<Device>();
            foreach (Device d in acarrofdevices)
            {
                mDevices.Add(d);
            }
            if (mDevices != null && mDevices.Count > 1)
            {
                //mDevices.Sort(delegate (Device d1, Device d2) { return d1.DeviceGroup.CompareTo(d2.DeviceGroup); });
                mDevices.Sort((d1, d2) => d1.DeviceGroup.CompareTo(d2.DeviceGroup));
            }

            for (int i = 1; i < mDevices.Count; i++)
            {
                if (!mDevices.ElementAt(i).DeviceGroup.Equals(mDevices.ElementAt(i - 1).DeviceGroup)) {
                    //сделать запись в бд
                }

            }

        }
    }

    public class Data{
        public Data() { }
    }
}