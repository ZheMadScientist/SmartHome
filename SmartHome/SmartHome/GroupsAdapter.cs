using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

using SmartHome.Model;
using Android.Content;
using Android.Util;

namespace SmartHome
{
    class GroupsAdapter : RecyclerView.Adapter
    {

        public List<Device> Devices { get; set; }
        public RecyclerView RecyclerView { get; set; }
        public View root { get; set; }

        Context c;

        int normalHeight = 0;

        public delegate void needToUpdate(int position);
        public static event needToUpdate update;

        public GroupsAdapter(List<Device> devices, Fragment fragment, RecyclerView recyclerView)
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
            var vh = (GroupsViewHolder)holder;
            c = vh.c;
            //  Fill up item info from questions list

            switch (Devices[position].DeviceGroup) {
                case (Constants.Constants.shaoimiBulb):
                    vh.Image.SetImageResource(Resource.Drawable.bulb);
                    vh.buttonTune.Text = "LIGHT: " + Devices[position].ExCondition;
                    break;

                case (Constants.Constants.shaoimiAqara):
                    vh.Image.SetImageResource(Resource.Drawable.bulb);
                    vh.buttonTune.Visibility = ViewStates.Gone;
                    break;

                case (Constants.Constants.bluetoothSpeaker):
                    vh.Image.SetImageResource(Resource.Drawable.audio);                    
                    vh.buttonTune.Text = "VOLUME: " + Devices[position].ExCondition;
                    break;
            }

            if (Devices[position].Favor)
            {
                vh.Bookmark.SetImageResource(Resource.Drawable.ic_bookmark_white_24dp);
            }
            else vh.Bookmark.SetImageResource(Resource.Drawable.ic_bookmark_border_white_24dp);


            vh.DeviceName.Text = Devices[position].DeviceName;
            vh.GroupsDescriptions.Text = Devices[position].DeviceDescription;
            vh.deviceState.Checked = Devices[position].Condition;
            vh.buttonDeleteDevice.Click += Delete_Clk;
            vh.deviceState.Click += stateSwitch;
            vh.Bookmark.Click += Favor_Clk;            
            vh.buttonTune.Click += Tune_Clk;
        }

        private void Tune_Clk(object sender, EventArgs e) {
            var button = (TextView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);
            //call dialog with circleview
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(c);
            LayoutInflater inflater = LayoutInflater.From(c);


            alert.SetTitle("Set value");
            alert.SetView(inflater.Inflate(Resource.Layout.alert_picker, null));           

            alert.SetPositiveButton("Set", (senderAlert, args) => {
                Devices[position].ExCondition = CirclePickerV.percent;
                Log.Debug("percentage", "" + CirclePickerV.percent);
                MainActivity.fireBall.SendData(new UserInfo());
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                alert.Dispose();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void Favor_Clk(object sender, EventArgs e) {
            var button = (ImageView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);

            Devices[position].Favor = !Devices[position].Favor;

            MainActivity.fireBall.SendData(new UserInfo());

            update(position);

            NotifyDataSetChanged();
        }


        private void Delete_Clk(object sender, EventArgs e)
        {
            var button = (TextView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);

            //RecyclerView.RemoveViewAt
            NotifyItemRemoved(position);

            if(Devices[position] != null)
                Devices.RemoveAt(position);

            MainActivity.fireBall.SendData(new UserInfo());

            update(position);

            NotifyDataSetChanged();
        }

        private void stateSwitch(object sender, EventArgs e) {
            var sw = (SwitchCompat)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)sw.Parent.Parent.Parent);

            Devices[position].Condition = sw.Checked;

            MainActivity.fireBall.SendData(new UserInfo());

            NotifyDataSetChanged();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.card, parent, false);
            normalHeight = layout.Height;
            return new GroupsViewHolder(layout);
        }
    }

    class GroupsViewHolder : RecyclerView.ViewHolder
    {
        public TextView DeviceName { get; private set; }
        public TextView GroupsDescriptions { get; private set; }
        public ImageView Image { get; private set; }
        public ImageView Bookmark { get; private set; }
        public TextView buttonDeleteDevice { get; set; }
        public TextView buttonTune { get; set; }
        public SwitchCompat deviceState { get; set; }
        public CirclePickerV picker { get; set; }

        public Context c;

        public GroupsViewHolder(View itemView) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.card_image);
            Bookmark = itemView.FindViewById<ImageView>(Resource.Id.card_image_bookmark);
            DeviceName = itemView.FindViewById<TextView>(Resource.Id.card_textView_name);
            GroupsDescriptions = itemView.FindViewById<TextView>(Resource.Id.card_textView);
            buttonDeleteDevice = itemView.FindViewById<TextView>(Resource.Id.card_button_delete);
            buttonTune = itemView.FindViewById<TextView>(Resource.Id.card_button_tune);
            deviceState = itemView.FindViewById<SwitchCompat>(Resource.Id.card_switch_compat);
            picker = itemView.FindViewById<CirclePickerV>(Resource.Id.circlePicker);
            c = itemView.Context;
        }
    }

}