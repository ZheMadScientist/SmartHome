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
    class ScriptsChildAdapter : RecyclerView.Adapter
    {
        public List<Device> Devices;

        public RecyclerView RecyclerView { get; set; }

        public delegate void needToUpdate(int position);
        public static event needToUpdate update;

        private Context context;

        public ScriptsChildAdapter(int position, RecyclerView recyclerView)
        {
            Devices = FireBall.scripts[position].Devices;
            RecyclerView = recyclerView;

        }

        public override int ItemCount => Devices.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = (ScriptsChildHolder)holder;
            context = vh.c;

            vh.deviceName.Text = Devices[position].DeviceName;
            if (!Devices[position].DeviceGroup.Equals(Constants.Constants.shaoimiAqara))
                vh.deviceValue.Text = Devices[position].ExCondition.ToString() + "%";

            vh.deviceState.Checked = Devices[position].Condition;
            vh.deviceState.Click += DeviceState_Click;
            vh.Card.Click += Card_Click;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            var button = (CardView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent);
            int ScriptPosition = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent.Parent.Parent);

            if (!Devices[position].DeviceGroup.Equals(Constants.Constants.shaoimiAqara))
            {
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(context);
                LayoutInflater inflater = LayoutInflater.From(context);

                alert.SetTitle("Set value");
                alert.SetView(inflater.Inflate(Resource.Layout.alert_picker, null));

                alert.SetPositiveButton("Set", (senderAlert, args) =>
                {
                    Devices[position].ExCondition = CirclePickerV.percent;

                    for (int i = 0; i < FireBall.scripts[ScriptPosition].Devices.Count; i++)
                    {
                        if (FireBall.scripts[ScriptPosition].Devices[i].Equals(Devices[position]))
                        {
                            FireBall.scripts[ScriptPosition].Devices[i] = Devices[position];
                        }
                    }

                    MainActivity.fireBall.SendData(new UserInfo());
                    update(ScriptPosition);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    alert.Dispose();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        private void DeviceState_Click(object sender, EventArgs e)
        {
            var button = (SwitchCompat)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);
            int ScriptPosition = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent.Parent.Parent.Parent.Parent);
            Devices[position].Condition = button.Checked;

            for (int i = 0; i < FireBall.scripts[ScriptPosition].Devices.Count; i++)
            {
                if (FireBall.scripts[ScriptPosition].Devices[i].Equals(Devices[position]))
                {
                    FireBall.scripts[ScriptPosition].Devices[i] = Devices[position];
                }
            }
            
            MainActivity.fireBall.SendData(new UserInfo());
            update(ScriptPosition);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.scripts_child_card, parent, false);
            return new ScriptsChildHolder(layout);
        }
    }

    class ScriptsChildHolder : RecyclerView.ViewHolder
    {
        public CardView Card { get; private set; }
        public TextView deviceName { get; private set; }
        public TextView deviceValue { get; set; }
        public SwitchCompat deviceState { get; set; }

        public Context c;

        public ScriptsChildHolder(View itemView) : base(itemView)
        {
            Card = itemView.FindViewById<CardView>(Resource.Id.child_scripts_card_device_item);
            deviceName = itemView.FindViewById<TextView>(Resource.Id.child_card_textview_name);
            deviceValue = itemView.FindViewById<TextView>(Resource.Id.child_card_textView_value);
            deviceState = itemView.FindViewById<SwitchCompat>(Resource.Id.child_card_switch_compat);
            c = itemView.Context;
        }
    }
}