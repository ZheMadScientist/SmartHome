using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Preferences;

using Firebase;
using Firebase.Auth;


namespace SmartHome
{
    [Service]
    class FireCfg : Service
    {
        public static FirebaseApp app;
        public static FirebaseAuth auth;

        public static bool readyB = false;


        public delegate void onReadyToWorkWithDB(FirebaseAuth a);
        public static event onReadyToWorkWithDB ready;

        public static ISharedPreferences sharedPref;

        public override void OnCreate()
        {
            base.OnCreate();

            Log.Debug("FireCfg", "OnCreate");
            //FirebaseDatabase.GetInstance(app).SetPersistenceEnabled(true);

            sharedPref = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            ISharedPreferencesEditor sharedEditor = sharedPref.Edit();


            var options = new FirebaseOptions.Builder()
                 .SetApplicationId("")
                 .SetApiKey("")
                 .SetDatabaseUrl("")
                 .SetStorageBucket("")
                .Build();

            if (app == null)
                app = FirebaseApp.InitializeApp(this, options, "SmartHome");

            auth = FirebaseAuth.GetInstance(app);
            Log.Debug("currentUser", "" + auth.CurrentUser);

            if (auth.CurrentUser == null)
            {
                Intent i = new Intent(this, typeof(FireAuthActivity));
                i.AddFlags(ActivityFlags.NewTask);
                StartActivity(i);
            }

            if (auth.CurrentUser != null) {
                Log.Debug("FireCfg", "auth = " + auth.CurrentUser);
                ready(auth);
                readyB = true;
                sharedEditor.PutString("user_email", auth.CurrentUser.Email);
                sharedEditor.Apply();
            }

        }
       
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
    }
}