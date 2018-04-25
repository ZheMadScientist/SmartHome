using System.Collections.Generic;

using SmartHome.Model;

using Firebase.Database;

using Newtonsoft.Json;
using Android.Util;

namespace SmartHome
{
    public class FireBall : Java.Lang.Object , IValueEventListener
    {
        FirebaseDatabase fDB;
        DatabaseReference fdRef;
        DatabaseReference requestRef;

        UserInfo userInfo;

        public delegate void onRequestComplete(UserInfo u);
        public static event onRequestComplete ready;

        public static List<Device> devicesRequested;
        public static List<Device> devicesFavorRequested;
        public static List<Script> scripts;

        public FireBall(int mode, UserInfo u) {
            //FirebaseDatabase.GetInstance(FireCfg.app).SetPersistenceEnabled(true);

            fDB = FirebaseDatabase.GetInstance(FireCfg.app);
            fdRef = fDB.Reference;
            requestRef = fDB.GetReference("users/" + FireCfg.auth.CurrentUser.Uid + "/UserInfo/obj");
            Log.Debug("reference = ", "https://smarthomerc.firebaseio.com/users/" + FireCfg.auth.CurrentUser.Uid + "/UserInfo");            

            AllDevicesF.onSend += SendData;

            switch (mode) {
                case (Constants.Constants.modeRequestData):
                    RequestData();
                    break;

                case (Constants.Constants.modeSendData):
                    SendData(u);
                    break;

                case (Constants.Constants.modeDeleteData):
                    DeleteData();
                    break;
            }
            devicesRequested = new List<Device>();
            devicesFavorRequested = new List<Device>();
            scripts = new List<Script>();
        }

        public void SendData(UserInfo u)
        {            
            fdRef.Child("users")
                .Child(FireCfg.auth.CurrentUser.Uid)
                .Child("UserInfo")
                .SetValue(JsonConvert.SerializeObject(u));
        }

        public void DeleteData()
        {
            fdRef.Child("users")
                .Child(FireCfg.auth.CurrentUser.Uid)
                .Child("UserInfo")
                .SetValue(null);
        }

        public void RequestData() {
            //requestRef.AddValueEventListener(this);
            fdRef.Child("users")
                .Child(FireCfg.auth.CurrentUser.Uid)
                .Child("UserInfo")
                .AddValueEventListener(this);
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (devicesRequested != null)
                devicesRequested.Clear();
            if (devicesFavorRequested != null)
                devicesFavorRequested.Clear();
            if (scripts != null)
                scripts.Clear();

            if (snapshot.Value != null)
            {
                userInfo = JsonConvert
                    .DeserializeObject<UserInfo>(snapshot.Value.ToString());

                Log.Debug("device count = ", userInfo.group.mDevices.Count + "");

                    foreach (Device d in userInfo.group.mDevices)
                    {
                                devicesRequested.Add(d);
                                if (d.Favor) {
                                    devicesFavorRequested.Add(d);
                                }
                    }

                scripts = userInfo.scripts;
                ready(userInfo);
            }            
        }

        public void OnCancelled(DatabaseError error)
        {
        }

    }
}