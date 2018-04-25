using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;
using SmartHome.Model;
using System;
using System.Collections.Generic;

namespace SmartHome
{

    class ScriptsAdapter : RecyclerView.Adapter
    {

        public List<Script> Scripts { get; set; }
        public RecyclerView RecyclerView { get; set; }
        public View root { get; set; }
        public ScriptsViewHolder viewHolder { get; set; }

        Context c;

        public delegate void needToUpdate(int position);
        public static event needToUpdate update;

        Color color = Color.White;

        public ScriptsAdapter(List<Script> scripts, Fragment fragment, RecyclerView recyclerView)
        {
            Scripts = scripts;
            RecyclerView = recyclerView;
            root = fragment.View;

            ScriptsChildAdapter.update += updateChild;
        }


        public override int ItemCount
        {
            get
            {
                return Scripts.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = (ScriptsViewHolder)holder;
            viewHolder = vh;
            c = vh.c;
            //  Fill up item info from questions list            
            color = Color.ParseColor("#" + FireBall.scripts[position].color.ToString("X"));

            vh.ScriptName.SetBackgroundColor(color);
            vh.ScriptName.Text = Scripts[position].name;
            vh.Plus.SetImageResource(Resource.Drawable.ic_mode_edit_white_24dp);
            vh.Plus.Click += addDeviceInDaScript;
            vh.Plus.Tag = position;
            vh.buttonDeleteScript.Click += Delete_Clk;
            vh.buttonTime.Click += SetTime;
            vh.scriptState.Checked = Scripts[position].isActive;
            vh.scriptState.CheckedChange += ScriptState;

            viewHolder.childRecycler.SetLayoutManager(new LinearLayoutManager(c));
            viewHolder.childRecycler.SetAdapter(new ScriptsChildAdapter(position, viewHolder.childRecycler));
            Log.Debug("OnBindViewHolder", "position = " + position);
        }

        private void addDeviceInDaScript(object sender, EventArgs e)
        {
            var button = (ImageView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);

            ListView listView;
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(c);
            LayoutInflater inflater = LayoutInflater.From(c);
            View v = inflater.Inflate(Resource.Layout.alert_listview, null);

            listView = v.FindViewById<ListView>(Resource.Id.ListView);
            listView.Adapter = new ListDialogAdapter(c, FireBall.devicesRequested, position);

            alert.SetTitle("Choose devices");
            alert.SetView(v);

            alert.SetPositiveButton("Edit", (senderAlert, args) =>
            {
                viewHolder.childRecycler.SetLayoutManager(new LinearLayoutManager(c));
                viewHolder.childRecycler.SetAdapter(new ScriptsChildAdapter(position, viewHolder.childRecycler));
                MainActivity.fireBall.SendData(new UserInfo());
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                alert.Dispose();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
            //диалог с листом девайсов
        }

        private void updateChild(int position)
        {
            if (viewHolder != null)
            {
                viewHolder.childRecycler.SetLayoutManager(new LinearLayoutManager(c));
                Log.Debug("updateChild", "position = " + position);
                viewHolder.childRecycler.SetAdapter(new ScriptsChildAdapter(position, viewHolder.childRecycler));
                viewHolder.childRecycler.ScrollToPosition(position);
                NotifyDataSetChanged();
            }
        }

        Calendar calendarNow;
        Calendar calendarSet;
        int hours = -1;
        int minutes = -1;
        private bool isTimer = false;
        private void SetTime(object sender, EventArgs e)
        {
            var button = (TextView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);

            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(c);
            LayoutInflater inflater = LayoutInflater.From(c);

            View v = inflater.Inflate(Resource.Layout.time_picker, null);
            TextView clockTimer = v.FindViewById<TextView>(Resource.Id.time_picker_button_clock);
            clockTimer.Click += ClockTimer_Click;

            alert.SetTitle("Set time");
            alert.SetView(v);

            alert.SetPositiveButton("Set", (senderAlert, args) =>
            {
                hours = CirclePickerV.hours;
                minutes = CirclePickerV.minutes;
                button.Text = hours + ":" + minutes;
                //set alarm manager
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                alert.Dispose();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private void ClockTimer_Click(object sender, EventArgs e)
        {
            var button = (TextView)sender;

            isTimer = !isTimer;

            button.Text = isTimer ? "MODE: TIMER" : "MODE: CLOCK";
        }

        private void ScriptState(object sender, EventArgs e)
        {
            var button = (SwitchCompat)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);

            Alarm alarm = new Alarm();

            calendarNow = Calendar.Instance;
            calendarSet = Calendar.Instance;

            if (position < 0)
                position = 0;
            Scripts[position].isActive = button.Checked;

            MainActivity.fireBall.SendData(new UserInfo());

            update(position);

            Log.Debug("ScriptState", "ScriptState");
            Log.Debug("ScriptPosition", "ScriptPosition");

            if (button.Checked)
            {
                Log.Debug("button.Checked", "button.Checked ");
                if (hours == -1 || minutes == -1)
                {
                    FireBall.scripts[position].isActive = false;
                    for (int i = 0; i < FireBall.devicesRequested.Count; i++)
                    {
                        foreach (Device d in FireBall.scripts[position].Devices)
                        {
                            if (FireBall.devicesRequested[i].Equals(d))
                            {
                                FireBall.devicesRequested[i] = d;
                            }
                        }
                    }
                    MainActivity.fireBall.SendData(new UserInfo());
                }
                else
                {
                    if (isTimer)
                    {
                        alarm.setAlarm(c, calendarNow.TimeInMillis, false, hours * 1000 * 60 * 60 + minutes * 1000 * 60, position);
                        Log.Debug("isTimer", "millis = " + CirclePickerV.hours * 1000 * 60 * 60 + CirclePickerV.minutes * 1000 * 60);
                    }
                    else
                    {
                        calendarSet.Set(Calendar.HourOfDay, hours);
                        calendarSet.Set(Calendar.Minute, minutes);
                        calendarSet.Set(Calendar.Second, 0);
                        calendarSet.Set(Calendar.Millisecond, 0);

                        if (calendarSet.CompareTo(calendarNow) <= 0)
                        {
                            calendarSet.Add(Calendar.Date, 1);
                        }
                        Log.Debug("calendarSet", "" + calendarSet.TimeInMillis);
                        Log.Debug("calendarNow", "" + calendarNow.TimeInMillis);

                        alarm.setAlarm(c, calendarNow.TimeInMillis, false, calendarSet.TimeInMillis - calendarNow.TimeInMillis, position);
                    }
                }
            }
            else
                alarm.cancelAlarm(c);

            NotifyDataSetChanged();
        }

        private void Delete_Clk(object sender, EventArgs e)
        {
            var button = (TextView)sender;
            int position = RecyclerView.GetChildLayoutPosition((View)button.Parent.Parent.Parent);

            try
            {
                if (Scripts[position] != null)
                    Scripts.RemoveAt(position);
            }
            catch (ArgumentOutOfRangeException) {
                Log.Debug("Catch", "Catch");
            }

            MainActivity.fireBall.SendData(new UserInfo());

            update(position);

            NotifyItemRemoved(position);
            NotifyDataSetChanged();
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.scripts_card, parent, false);
            return new ScriptsViewHolder(layout);
        }


    }

    class ScriptsViewHolder : RecyclerView.ViewHolder
    {
        public TextView ScriptName { get; private set; }
        public ImageView Plus { get; private set; }
        public TextView buttonDeleteScript { get; set; }
        public TextView buttonTime { get; set; }
        public SwitchCompat scriptState { get; set; }
        public CirclePickerV picker { get; set; }

        public Context c;

        public RecyclerView childRecycler { get; private set; }

        public ScriptsViewHolder(View itemView) : base(itemView)
        {
            Plus = itemView.FindViewById<ImageView>(Resource.Id.scripts_card_plus_image);
            ScriptName = itemView.FindViewById<TextView>(Resource.Id.scripts_card_textView_name);
            buttonDeleteScript = itemView.FindViewById<TextView>(Resource.Id.scripts_card_button_delete);
            buttonTime = itemView.FindViewById<TextView>(Resource.Id.scripts_card_button_time);
            scriptState = itemView.FindViewById<SwitchCompat>(Resource.Id.scripts_card_switch_compat);
            picker = itemView.FindViewById<CirclePickerV>(Resource.Id.circlePicker);
            c = itemView.Context;
            childRecycler = itemView.FindViewById<RecyclerView>(Resource.Id.scripts_childRecyclerView);

            if (itemView.LayoutParameters.Height > 300)
            {
                itemView.LayoutParameters.Height = 300;
                Log.Debug("ScriptsViewHolder", "Height = " + itemView.Height);
            }
        }

    }
}