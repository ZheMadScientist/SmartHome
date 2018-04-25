
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Firebase.Auth;
using SmartHome.Model;

namespace SmartHome
{
    [BroadcastReceiver]
    public class Alarm : BroadcastReceiver
    {

        FireBall fireBall;
        bool isRequestNeeded = true;

        int scriptPos = -1;
        public override void OnReceive(Context context, Intent intent)
        {
            Intent startservice = new Intent(context, typeof(FireCfg));
            context.StartService(startservice);

            scriptPos = intent.GetIntExtra("position", 0);

            FireCfg.ready += readyToDB;

            if (FireCfg.readyB) {
                readyToDB(FireCfg.auth);
                Log.Debug("FireCfg.readyB", "FireCfg.readyB");
            }

            Log.Debug("OnReceive", "OnReceive " + "position = " + scriptPos);
            Toast.MakeText(context, "Received intent!", ToastLength.Long).Show();
        }

        private void readyToDB(FirebaseAuth a)
        {
            fireBall = new FireBall(Constants.Constants.modeRequestData, null);
            FireBall.ready += onRequestComplete;
            Log.Debug("OnReceive", "readyToDB " + "position = " + scriptPos);            
        }

        private void onRequestComplete(UserInfo u) {
            if (isRequestNeeded)
            {
                Log.Debug("onRequestComplete", "onRequestComplete");
                FireBall.scripts[scriptPos].isActive = false;
                for (int i = 0; i < FireBall.devicesRequested.Count; i++)
                {
                    foreach (Device d in FireBall.scripts[scriptPos].Devices)
                    {
                        if (FireBall.devicesRequested[i].Equals(d))
                        {
                            FireBall.devicesRequested[i] = d;
                        }
                    }
                }
                fireBall.SendData(new UserInfo());
                isRequestNeeded = false;
            }
        }

        public void setAlarm(Context c, long time, bool isRepeated, long interval, int position) {
            Log.Debug("setAlarm", "setAlarm ");
            AlarmManager am = (AlarmManager)c.GetSystemService(Context.AlarmService);
            Intent i = new Intent(c, typeof(Alarm));
            i.PutExtra("position", position);
            PendingIntent pi = PendingIntent.GetBroadcast(c, 0, i, 0);
            if (isRepeated) {
                //am.SetRepeating(AlarmType.RtcWakeup, time + interval, , pi);
            }
            else
            {
                Log.Debug("setAlarm", "TIMER ");
                am.Set(AlarmType.RtcWakeup, time + interval, pi);
                Log.Debug("time", time + "");
                Log.Debug("interval", interval + "");
            }
        }


        public void cancelAlarm(Context c) {
            Log.Debug("cancelAlarm", "cancelAlarm");
            Intent intent = new Intent(c, typeof(Alarm));
            PendingIntent sender = PendingIntent.GetBroadcast(c, 0, intent, 0);
            AlarmManager am = (AlarmManager)c.GetSystemService(Context.AlarmService);
            am.Cancel(sender);
        }

    }
}