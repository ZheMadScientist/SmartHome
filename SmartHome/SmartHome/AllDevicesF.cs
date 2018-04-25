using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using SmartHome.Model;
using System;
using static Android.Views.View;

namespace SmartHome
{
    public class AllDevicesF : Fragment, IOnClickListener, IOnFocusChangeListener
    {
        View root;
        RecyclerView recyclerView;
        LinearLayoutManager linManager;
        FloatingActionButton fab;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //AddDeviceActivity.upd += updateData;
            GroupsAdapter.update += updateData;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);            

            root = inflater.Inflate(Resource.Layout.all_devices_layout, container, false);

            var swipeContainer = root.FindViewById<SwipeRefreshLayout>(Resource.Id.swipeContainer);
            swipeContainer.SetColorSchemeResources(Resource.Color.colorPrimaryDark);
            swipeContainer.Refresh += SwipeContainer_Refresh;

            fab = root.FindViewById<FloatingActionButton>(Resource.Id.all_devices_fab);
            fab.SetOnClickListener(this);

            recyclerView = root.FindViewById<RecyclerView>(Resource.Id.all_devices_recyclerView);

            linManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(linManager);
            recyclerView.SetAdapter(new GroupsAdapter(FireBall.devicesRequested, this, recyclerView));

            recyclerView.ScrollChange += RecyclerView_ScrollChange;

            return root;
           
        }

        EditText deviceName, deviceDescription;
        Spinner spinner;
        ImageView image;

        int spinner_item = -1;

        public delegate void sendData(UserInfo u);
        public static event sendData onSend;

        private void addDeviceDialog()
        {
           
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Context);
            LayoutInflater inflater = LayoutInflater.From(Context);
            View v = inflater.Inflate(Resource.Layout.add_device_layout, null);

            Device createdDevice;

            deviceName = v.FindViewById<EditText>(Resource.Id.add_device_name);
            deviceDescription = v.FindViewById<EditText>(Resource.Id.add_device_description);

            image = v.FindViewById<ImageView>(Resource.Id.add_device_card_image);

            spinner = v.FindViewById<Spinner>(Resource.Id.add_device_spinner);

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    Context, Resource.Array.devices_array, Resource.Layout.spinner_item);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            alert.SetView(v);
            alert.SetTitle("Add script");

            alert.SetPositiveButton("Create", (senderAlert, args) => {
                if (checkDaString(deviceName.Text, deviceDescription.Text))
                {                    
                    if (spinner_item != 1) { createdDevice = new Device(deviceName.Text, deviceDescription.Text, spinner.SelectedItem.ToString(), false, true, 0, false); }
                    else { createdDevice = new Device(deviceName.Text, deviceDescription.Text, spinner.SelectedItem.ToString(), false, false, 0, false); }
                    if (isDeviseOk(createdDevice))
                    {
                        FireBall.devicesRequested.Add(createdDevice);
                        onSend(new UserInfo());
                        updateData(0);
                    }
                }
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                alert.Dispose();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private bool checkDaString(string name, string description)
        {
            bool isNameOk, isDescrOk;
            isNameOk = !TextUtils.IsEmpty(name) && name.Length <= 15;
            isDescrOk = description.Length <= 30;
            if (!isNameOk) { Toast.MakeText(Context, "Name can not be empty or contain more than 15 letters", ToastLength.Short).Show(); }
            if (!isDescrOk) { Toast.MakeText(Context, "Description can not contain more than 30 letters", ToastLength.Short).Show(); }
            return isNameOk && isDescrOk;
        }

        private bool isDeviseOk(Device createdDevice) {
            bool isOk = true;

            foreach (Device d in FireBall.devicesRequested) {
                if (d.Equals(createdDevice)) {
                    isOk = false;
                    Toast.MakeText(Context, "The device has already been created", ToastLength.Short).Show();
                }
            }

            return isOk;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            spinner_item = e.Position;
            switch (e.Position)
            {
                case (0):
                    image.SetImageResource(Resource.Drawable.bulb);
                    break;

                case (1):
                    image.SetImageResource(Resource.Drawable.bulb);
                    break;

                case (2):
                    image.SetImageResource(Resource.Drawable.audio);
                    break;
            }

        }

        private void RecyclerView_ScrollChange(object sender, ScrollChangeEventArgs e)
        {
            if (linManager.FindFirstCompletelyVisibleItemPosition() > 0)
            {
                fab.Hide();
            }
            else fab.Show();
        }

        public override void OnResume()
        {
            base.OnResume();
            
        }

        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            updateData(0);
            (sender as SwipeRefreshLayout).Refreshing = false;
        }


        private void updateData(int position) {
            //recyclerView.RemoveAllViews();
            MainActivity.fireBall.RequestData();
            recyclerView.SetAdapter(new GroupsAdapter(FireBall.devicesRequested, this, recyclerView));
            recyclerView.ScrollToPosition(position);
        }

    

        public void OnClick(View v)
        {
            switch (v.Id) {
                case (Resource.Id.all_devices_fab):
                    addDeviceDialog();
                    break;

            }
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            if (v.Id == root.Id && hasFocus) {
                updateData(0);
            }
        }

     
    }
}