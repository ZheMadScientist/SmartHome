using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome
{
    class ListDialogAdapter : BaseAdapter<Device>
    {

        List<Device> items;
        Context context;
        int scriptPosition = 0;

        public ListDialogAdapter(Context context, List<Device> items, int scrPos)
        {
            this.context = context;
            this.items = items;
            scriptPosition = scrPos;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Device this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            ImageView ic;
            TextView tv;
            CheckBox cb;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item, parent, false);
            }

            ic = view.FindViewById<ImageView>(Resource.Id.list_item_icon);
            switch (items[position].DeviceGroup) {
                case Constants.Constants.shaoimiBulb:
                    ic.SetImageResource(Resource.Drawable.ic_lightbulb_outline_black_24dp);
                    break;
                case Constants.Constants.shaoimiAqara:
                    ic.SetImageResource(Resource.Drawable.ic_lightbulb_outline_black_24dp);
                    break;
                case Constants.Constants.bluetoothSpeaker:
                    ic.SetImageResource(Resource.Drawable.ic_speaker_black_24dp);
                    break;
            }

            
            tv = view.FindViewById<TextView>(Resource.Id.list_item_text);
            tv.Text = items[position].DeviceName;
            cb = view.FindViewById<CheckBox>(Resource.Id.checkbox_list_item);
            cb.Tag = position;

            foreach (Device d in FireBall.scripts[scriptPosition].Devices.ToList()) {
                if (d.DeviceName.Equals(items[position].DeviceName)) {
                    cb.Checked = true;
                }
            }
            Log.Debug("GetView", "GetView");
            cb.Click += Cb_Click;
            return view;
        }

        private void Cb_Click(object sender, System.EventArgs e)
        {
            var box = (CheckBox)sender;
            int position = (int)box.Tag;
            if (box.Checked)
            {
                FireBall.scripts[scriptPosition].Devices.Add(FireBall.devicesRequested[position]);
            }
            else
            {
                FireBall.scripts[scriptPosition].Devices.Remove(FireBall.devicesRequested[position]);
            }
            for (int i = 1; i < FireBall.scripts[scriptPosition].Devices.Count; i++) {
                if (FireBall.scripts[scriptPosition].Devices[i - 1].Equals(FireBall.scripts[scriptPosition].Devices[i]))
                    FireBall.scripts[scriptPosition].Devices.RemoveAt(i);
            }
        }       

    }
}