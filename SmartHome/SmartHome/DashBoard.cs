using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using System.Linq;
using static Android.Views.View;

namespace SmartHome
{
    [Activity(Label = "DashBoard")]
    public class DashBoard : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        TextView txtWelcome;
        EditText input_new_password;
        Button btnChangePass, btnLogout, btnDeleteUser;
        RelativeLayout activity_dashboard;

        FirebaseAuth auth;
        FirebaseUser user;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DashBoard);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(FireCfg.app);

            //View
            txtWelcome = FindViewById<TextView>(Resource.Id.dashboard_welcome);
            input_new_password = FindViewById<EditText>(Resource.Id.dashboard_newpassword);
            btnChangePass = FindViewById<Button>(Resource.Id.dashboard_btn_change_pass);
            btnLogout = FindViewById<Button>(Resource.Id.dashboard_btn_logout);
            btnDeleteUser = FindViewById<Button>(Resource.Id.dashboard_btn_delete_user);
            activity_dashboard = FindViewById<RelativeLayout>(Resource.Id.activity_dashboard);

            btnChangePass.SetOnClickListener(this);
            btnLogout.SetOnClickListener(this);
            btnDeleteUser.SetOnClickListener(this);

            //Check session
            if (auth.CurrentUser != null)
                txtWelcome.Text = "Welcome , " + auth.CurrentUser.Email;

            user = auth.CurrentUser;
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.dashboard_btn_change_pass)
                ChangePassword(input_new_password.Text);
            if (v.Id == Resource.Id.dashboard_btn_logout)
                LogoutUser();
            if (v.Id == Resource.Id.dashboard_btn_delete_user)
                DeleteUser();
        }

        private void LogoutUser()
        {
            auth.SignOut();
            if (auth.CurrentUser == null)
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
        }

        private void ChangePassword(string newPassword)
        {
            if (IsValidPassword(newPassword))
            {
                FirebaseUser user = auth.CurrentUser;
                user.UpdatePassword(newPassword)
                    .AddOnCompleteListener(this);
            }
        }

        private void DeleteUser() {
            Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);

            alert.SetTitle("Confirm delete");
            alert.SetMessage("Do you want to delete your accont?");
          
            alert.SetPositiveButton("Delete account", (senderAlert, args) => {
                
                AuthCredential credential = EmailAuthProvider
                    .GetCredential(user.Email, "1234");

                user.ReauthenticateAsync(credential);

                user.DeleteAsync();

                Toast.MakeText(this, "Deleted", ToastLength.Short).Show();

                LogoutUser();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                alert.Dispose();
            });

            Dialog dialog = alert.Create();
            dialog.Show();

        }

        private bool IsValidPassword(string target)
        {
            return !TextUtils.IsEmpty(target) && !target.Contains(' ');
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Snackbar snackBar = Snackbar.Make(activity_dashboard, "Success", Snackbar.LengthShort);
                snackBar.Show();
                Finish();
            }
        }
    }
}