using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using static Android.Views.View;

namespace SmartHome
{
    public class DashboardF : Fragment, IOnFocusChangeListener
    {        
        View root;
        RecyclerView recyclerView;
        RecyclerView.Adapter dAdapter;
        StaggeredGridLayoutManager stlm;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DashboardAdapter.update += updateData;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            root = inflater.Inflate(Resource.Layout.dashboard_fragment_layout, container, false);

            var swipeContainer = root.FindViewById<SwipeRefreshLayout>(Resource.Id.dashboard_swipeContainer);
            swipeContainer.SetColorSchemeResources(Resource.Color.colorPrimaryDark);
            swipeContainer.Refresh += SwipeContainer_Refresh;

            recyclerView = root.FindViewById<RecyclerView>(Resource.Id.dashboard_recyclerView);
            stlm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            recyclerView.SetLayoutManager(stlm);
            dAdapter = new DashboardAdapter(FireBall.devicesFavorRequested, this, recyclerView);
            recyclerView.SetAdapter(dAdapter); 
            return root;
        }


       

        void SwipeContainer_Refresh(object sender, EventArgs e)
        {
            updateData();
            (sender as SwipeRefreshLayout).Refreshing = false;
        }

        private void updateData()
        {
            
            MainActivity.fireBall.RequestData();
            recyclerView.SetLayoutManager(new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical));
            recyclerView.SetAdapter(new DashboardAdapter(FireBall.devicesFavorRequested, this, recyclerView));

        }

        public void OnFocusChange(View v, bool hasFocus)
        {
            if (v.Id == root.Id && hasFocus)
            {
                updateData();
            }
        }

    }
}