using Android.App;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using SmartHome.Model;
using System;
using System.Collections.Generic;

namespace SmartHome
{
    class DashboardAdapter : RecyclerView.Adapter
    {

        public List<Device> Devices { get; set; }
        public RecyclerView RecyclerView { get; set; }
        public View root { get; set; }

        Context c;

        int normalHeight = 0;

        public delegate void needToRefresh();
        public static event needToRefresh update;

        public DashboardAdapter(List<Device> devices, Fragment fragment, RecyclerView recyclerView)
        {
            Devices = devices;
            RecyclerView = recyclerView;
            root = fragment.View;
        }


        public override int ItemCount
        {
            get
            {
                return Devices.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = (DashboardViewHolder)holder;

            c = vh.c;

            //  Fill up item info from questions list

            switch (Devices[position].DeviceGroup)
            {
                case (Constants.Constants.shaoimiBulb):
                    vh.Icon.SetImageResource(Resource.Drawable.ic_brightness_7_white_24dp);                    
                    break;

                case (Constants.Constants.bluetoothSpeaker):
                    vh.Icon.SetImageResource(Resource.Drawable.ic_volume_up_white_24dp);
                    break;
            }


            vh.DeviceName.Text = Devices[position].DeviceName;
            vh.DeviceDescriptions.Text = Devices[position].DeviceDescription;

            if (!Devices[position].DeviceGroup.Equals(Constants.Constants.shaoimiAqara))
                vh.Value.Text = Devices[position].ExCondition.ToString() + "%";

            vh.OnOff.Text = Devices[position].Condition ? "ON" : "OFF";

            vh.OnOff.Click += OnOff_Clk;
            vh.Card.Click += Tune_Clk;
        }

        private void OnOff_Clk(object sender, EventArgs e) {
            var button = (TextView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);

            Devices[position].Condition = !Devices[position].Condition;
            MainActivity.fireBall.SendData(new UserInfo());
            update();
        }

        private void Tune_Clk(object sender, EventArgs e)
        {
            //call dialog with circleview
            var button = (CardView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent);
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(c);
            LayoutInflater inflater = LayoutInflater.From(c);


            alert.SetTitle("Set value");
            alert.SetView(inflater.Inflate(Resource.Layout.alert_picker, null));

            alert.SetPositiveButton("Set", (senderAlert, args) => {
                Devices[position].ExCondition = CirclePickerV.percent;
                MainActivity.fireBall.SendData(new UserInfo());
                update();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                alert.Dispose();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

       

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.dashboard_card, parent, false);
            normalHeight = layout.Height;
            return new DashboardViewHolder(layout);
        }
    }

    class DashboardViewHolder : RecyclerView.ViewHolder
    {
        public CardView Card { get; private set; }
        public TextView DeviceName { get; private set; }
        public TextView DeviceDescriptions { get; private set; }
        public TextView Value { get; private set; }
        public TextView OnOff { get; private set; }
        public ImageView Icon { get; private set; }

        public Context c;

        public DashboardViewHolder(View itemView) : base(itemView)
        {
            Card = itemView.FindViewById<CardView>(Resource.Id.dashboard_card_root);
            Icon = itemView.FindViewById<ImageView>(Resource.Id.dashboard_card_icon);
            DeviceName = itemView.FindViewById<TextView>(Resource.Id.dashboard_card_name);
            DeviceDescriptions = itemView.FindViewById<TextView>(Resource.Id.dashboard_card_description);
            Value = itemView.FindViewById<TextView>(Resource.Id.dashboard_card_value);
            OnOff = itemView.FindViewById <TextView>(Resource.Id.dashboard_card_onoff);
            c = itemView.Context;
        }
    }

}