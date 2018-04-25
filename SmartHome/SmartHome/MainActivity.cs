using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Content;

using Firebase.Auth;
using Android.Content.PM;
using Android.Net;
using SmartHome.Model;
using Android.Graphics;

namespace SmartHome
{
    [MetaData("android.app.shortcuts", Resource = "@menu/shortcuts")]
    [Activity(Name = "com.vk.chetkiyperets.SmartHome.MainActivity", Label = "SmartHome", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation,
    ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;        
        TextView userEmail;
        ProgressBar progressBar;

        bool isInitNeeded = true;

        public static FireBall fireBall;

        int startFragment = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);            

            // Init toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.app_bar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetTitle(Resource.String.ApplicationName);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            // Attach item selected handler to navigation view
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            
            userEmail = navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.nav_text_small);

            // Create ActionBarDrawerToggle button and add it to the toolbar
            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);            

            if (!isOnline()) {
                Toast.MakeText(this, "Сannot connect to the Internet", ToastLength.Short).Show();
                //Finish();
            }

            if (Constants.Constants.action_alldevices.Equals(Intent.Action))
                startFragment = 1;
            if (Constants.Constants.action_scripts.Equals(Intent.Action))
                startFragment = 2;

            FireBall.ready += FireBall_ready;
            progressBar.Visibility = ViewStates.Visible;
            progressBar.IndeterminateTintList = Android.Content.Res.ColorStateList.ValueOf(Color.White);
        }

        private bool isOnline() {
            ConnectivityManager cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo netInfo = cm.ActiveNetworkInfo;
            return netInfo != null && netInfo.IsConnectedOrConnecting;
        }

        private void startServiceThread()
        {
            new Java.Lang.Thread(() =>
            {
                Intent startservice = new Intent(this, typeof(FireCfg));

                RunOnUiThread(() => StartService(startservice));
            }).Start();
        }

        protected override void OnResume()
        {            
            base.OnResume();
            SupportActionBar.SetTitle(Resource.String.ApplicationName);
            startServiceThread();
            FireCfg.ready += readyToDB;
            FireAuthActivity.readyA += readyToDB;
            FireBall.ready += FireBall_ready;
        }

        private void FireBall_ready(UserInfo u)
        {
            if (isInitNeeded)
            {
                progressBar.Visibility = ViewStates.Gone;
                var ft = FragmentManager.BeginTransaction();
                ft.AddToBackStack(null);
                switch (startFragment) {
                    case 0:
                        ft.Add(Resource.Id.HomeFrameLayout, new DashboardF());
                        ft.CommitAllowingStateLoss();
                        break;
                    case 1:
                        ft.Add(Resource.Id.HomeFrameLayout, new AllDevicesF());
                        ft.CommitAllowingStateLoss();
                        break;
                    case 2:
                        ft.Add(Resource.Id.HomeFrameLayout, new ScriptsFragment());
                        ft.CommitAllowingStateLoss();
                        break;
                }
               
                isInitNeeded = false;
            }
        
        }

        private void readyToDB(FirebaseAuth a) {
            Log.Debug("Preferences", FireCfg.sharedPref.GetString("user_email", "SmartHome"));
            userEmail.Text = FireCfg.sharedPref.GetString("user_email", "SmartHome");
            fireBall = new FireBall(Constants.Constants.modeRequestData, null);
        }

        protected override void OnStop()
        {
            base.OnStop();
            StopService(new Intent(this, typeof(FireCfg)));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopService(new Intent(this, typeof(FireCfg)));
        }

        //define action for navigation menu selection
        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            var fm = FragmentManager.BeginTransaction();
            fm.AddToBackStack(null);

            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_dashboard):
                    fm.Add(Resource.Id.HomeFrameLayout, new DashboardF());
                    break;

                case (Resource.Id.nav_device_list):
                    fm.Add(Resource.Id.HomeFrameLayout, new AllDevicesF());
                    break;

                case (Resource.Id.nav_scripts):
                    fm.Add(Resource.Id.HomeFrameLayout, new ScriptsFragment());
                    //fm.Replace(Resource.Id.HomeFrameLayout, new ScriptsFragment());
                    break;

            }
            fm.Commit();
            // Close drawer
            drawerLayout.CloseDrawers();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_menu, menu);           
            return base.OnCreateOptionsMenu(menu);
        }

        //define action for tolbar icon press
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
               
                case Resource.Id.action_auth:
                    StartActivity(typeof(DashBoard));
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        //to avoid direct app exit on backpreesed and to show fragment from stack
        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount != 0)
            {
                FragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }
}

