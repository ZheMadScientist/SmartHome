using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using SmartHome.Model;
using System;
using static Android.Views.View;

namespace SmartHome
{
    public class ScriptsFragment : Fragment, IOnClickListener, IOnFocusChangeListener
    {
        View root;
        FloatingActionButton fab;
        RecyclerView recyclerView;
        LinearLayoutManager linManager;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ScriptsAdapter.update += updateData;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            root = inflater.Inflate(Resource.Layout.scripts_fragment, container, false);

            var swipeContainer = root.FindViewById<SwipeRefreshLayout>(Resource.Id.scripts_swipeContainer);
            swipeContainer.SetColorSchemeResources(Resource.Color.colorPrimaryDark);
            swipeContainer.Refresh += SwipeContainer_Refresh;

            fab = root.FindViewById<FloatingActionButton>(Resource.Id.scripts_fab);
            fab.SetOnClickListener(this);

            recyclerView = root.FindViewById<RecyclerView>(Resource.Id.scripts_recyclerView);

            linManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(linManager);
            recyclerView.SetAdapter(new ScriptsAdapter(FireBall.scripts, this, recyclerView));

            recyclerView.ScrollChange += RecyclerView_ScrollChange;

            return root;
        }

        private void RecyclerView_ScrollChange(object sender, ScrollChangeEventArgs e)
        {
            if (linManager.FindFirstCompletelyVisibleItemPosition() > 0)
            {
                fab.Hide();
            }
            else fab.Show();

        }

        private void updateData(int position)
        {
            //recyclerView.RemoveAllViews();
            MainActivity.fireBall.RequestData();
            recyclerView.SetAdapter(new ScriptsAdapter(FireBall.scripts, this, recyclerView));
            recyclerView.ScrollToPosition(position);
        }

        private void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            updateData(0);
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        int scriptColor;
        Button orange, red, blue, green, teal, grey, black;
        private void addScriptDialog() {
            scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_orange);

            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(Context);
            LayoutInflater inflater = LayoutInflater.From(Context);

            View v = inflater.Inflate(Resource.Layout.alert_new_script, null);
            EditText userInput = v.FindViewById<EditText>(Resource.Id.alert_add_script_name);

            orange = v.FindViewById<Button>(Resource.Id.script_color_orange_button);
            orange.SetOnClickListener(this);
            red = v.FindViewById<Button>(Resource.Id.script_color_red_button);
            red.SetOnClickListener(this);
            blue = v.FindViewById<Button>(Resource.Id.script_color_blue_button);
            blue.SetOnClickListener(this);
            green = v.FindViewById<Button>(Resource.Id.script_color_green_button);
            green.SetOnClickListener(this);
            teal = v.FindViewById<Button>(Resource.Id.script_color_teal_button);
            teal.SetOnClickListener(this);
            grey = v.FindViewById<Button>(Resource.Id.script_color_grey_button);
            grey.SetOnClickListener(this);
            black = v.FindViewById<Button>(Resource.Id.script_color_black_button);
            black.SetOnClickListener(this);

            orange.ScaleX = 1.2F;
            orange.ScaleY = 1.2F;

            userInput.Hint = "Enter script name";

            alert.SetView(v);
            alert.SetTitle("Add script");

            alert.SetPositiveButton("Create", (senderAlert, args) => {
                if (checkName(userInput.Text))
                {
                    FireBall.scripts.Add(new Script(userInput.Text, scriptColor));
                    MainActivity.fireBall.SendData(new UserInfo());
                    updateData(0);
                }
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                alert.Dispose();
            });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

        private bool checkName(string target) {
            bool ans = !TextUtils.IsEmpty(target) && target.Length <= 48;
            if (!ans) { Toast.MakeText(Context, "Name can not be empty or contain more than 48 letters", ToastLength.Short).Show(); }
            return ans;
        }

        public void OnClick(View v)
        {            
            switch (v.Id)
            {
                case (Resource.Id.scripts_fab):
                    addScriptDialog();
                    break;               

                case (Resource.Id.script_color_orange_button):
                    scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_orange);
                    Log.Debug("OnClick scriptColor = ", "" + scriptColor);
                    v.ScaleX = 1.2F;
                    v.ScaleY = 1.2F;

                    red.ScaleX = 1;
                    red.ScaleY = 1;
                    blue.ScaleX = 1;
                    blue.ScaleY = 1;
                    green.ScaleX = 1;
                    green.ScaleY = 1;
                    teal.ScaleX = 1;
                    teal.ScaleY = 1;
                    grey.ScaleX = 1;
                    grey.ScaleY = 1;
                    black.ScaleX = 1;
                    black.ScaleY = 1;
                    break;

                case (Resource.Id.script_color_red_button):
                    scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_red);
                    Log.Debug("OnClick scriptColor = ", "" + scriptColor);
                    v.ScaleX = 1.2F;
                    v.ScaleY = 1.2F;

                    orange.ScaleX = 1;
                    orange.ScaleY = 1;
                    blue.ScaleX = 1;
                    blue.ScaleY = 1;
                    green.ScaleX = 1;
                    green.ScaleY = 1;
                    teal.ScaleX = 1;
                    teal.ScaleY = 1;
                    grey.ScaleX = 1;
                    grey.ScaleY = 1;
                    black.ScaleX = 1;
                    black.ScaleY = 1;
                    break;

                case (Resource.Id.script_color_blue_button):
                    scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_blue);
                    Log.Debug("OnClick scriptColor = ", "" + scriptColor);
                    v.ScaleX = 1.2F;
                    v.ScaleY = 1.2F;

                    orange.ScaleX = 1;
                    orange.ScaleY = 1;
                    red.ScaleX = 1;
                    red.ScaleY = 1;
                    green.ScaleX = 1;
                    green.ScaleY = 1;
                    teal.ScaleX = 1;
                    teal.ScaleY = 1;
                    grey.ScaleX = 1;
                    grey.ScaleY = 1;
                    black.ScaleX = 1;
                    black.ScaleY = 1;
                    break;

                case (Resource.Id.script_color_green_button):
                    scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_green);
                    Log.Debug("OnClick scriptColor = ", "" + scriptColor);
                    v.ScaleX = 1.2F;
                    v.ScaleY = 1.2F;

                    orange.ScaleX = 1;
                    orange.ScaleY = 1;
                    red.ScaleX = 1;
                    red.ScaleY = 1;
                    blue.ScaleX = 1;
                    blue.ScaleY = 1;
                    teal.ScaleX = 1;
                    teal.ScaleY = 1;
                    grey.ScaleX = 1;
                    grey.ScaleY = 1;
                    black.ScaleX = 1;
                    black.ScaleY = 1;
                    break;

                case (Resource.Id.script_color_teal_button):
                    scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_teal);
                    Log.Debug("OnClick scriptColor = ", "" + scriptColor);
                    v.ScaleX = 1.2F;
                    v.ScaleY = 1.2F;

                    orange.ScaleX = 1;
                    orange.ScaleY = 1;
                    red.ScaleX = 1;
                    red.ScaleY = 1;
                    blue.ScaleX = 1;
                    blue.ScaleY = 1;
                    green.ScaleX = 1;
                    green.ScaleY = 1;
                    grey.ScaleX = 1;
                    grey.ScaleY = 1;
                    black.ScaleX = 1;
                    black.ScaleY = 1;
                    break;

                case (Resource.Id.script_color_grey_button):
                    scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_grey);
                    Log.Debug("OnClick scriptColor = ", "" + scriptColor);
                    v.ScaleX = 1.2F;
                    v.ScaleY = 1.2F;

                    orange.ScaleX = 1;
                    orange.ScaleY = 1;
                    red.ScaleX = 1;
                    red.ScaleY = 1;
                    blue.ScaleX = 1;
                    blue.ScaleY = 1;
                    green.ScaleX = 1;
                    green.ScaleY = 1;
                    teal.ScaleX = 1;
                    teal.ScaleY = 1;
                    black.ScaleX = 1;
                    black.ScaleY = 1;
                    break;

                case (Resource.Id.script_color_black_button):
                    scriptColor = ContextCompat.GetColor(Context, Resource.Color.script_color_black);
                    Log.Debug("OnClick scriptColor = ", "" + scriptColor);
                    v.ScaleX = 1.2F;
                    v.ScaleY = 1.2F;

                    orange.ScaleX = 1;
                    orange.ScaleY = 1;
                    red.ScaleX = 1;
                    red.ScaleY = 1;
                    blue.ScaleX = 1;
                    blue.ScaleY = 1;
                    green.ScaleX = 1;
                    green.ScaleY = 1;
                    teal.ScaleX = 1;
                    teal.ScaleY = 1;
                    grey.ScaleX = 1;
                    grey.ScaleY = 1;
                    break;

            }
        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            if (v.Id == root.Id && hasFocus)
            {
                updateData(0);
            }
        }
    }
}