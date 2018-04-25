using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Firebase.Auth;
using static Android.Views.View;
using Android.Gms.Tasks;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Util;

namespace SmartHome
{
    [Activity(Label = "ForgotPassword")]
    public class ForgotPassword : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        private EditText input_email;
        private Button btnResetPass;
        private TextView btnBack;
        private RelativeLayout activity_forgot;


        FirebaseAuth auth;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ForgotPassword);

            //Init Firebase
            auth = FirebaseAuth.GetInstance(FireCfg.app);

            //View
            input_email = FindViewById<EditText>(Resource.Id.forgot_email);
            btnResetPass = FindViewById<Button>(Resource.Id.forgot_btn_reset);
            btnBack = FindViewById<TextView>(Resource.Id.forgot_btn_back);
            activity_forgot = FindViewById<RelativeLayout>(Resource.Id.activity_forgot);

            btnResetPass.SetOnClickListener(this);
            btnBack.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.forgot_btn_back)
            {
                //StartActivity(new Intent(this, typeof(MainActivity)));
                Finish();
            }
            else if (v.Id == Resource.Id.forgot_btn_reset)
            {
                ResetPassword(input_email.Text);
            }
        }

        private void ResetPassword(string email)
        {
            if (IsValidEmail(email))
            {
                auth.SendPasswordResetEmail(email)
                    .AddOnCompleteListener(this, this);
            }
        }

        private bool IsValidEmail(string target)
        {
            return !TextUtils.IsEmpty(target) && Patterns.EmailAddress.Matcher(target).Matches() && !target.Contains(' ');
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == false)
            {
                Snackbar snackBar = Snackbar.Make(activity_forgot, "Reset password failed", Snackbar.LengthShort);
                snackBar.Show();
            }
            else
            {
                Snackbar snackBar = Snackbar.Make(activity_forgot, "Reset password link sent to email : " + input_email.Text, Snackbar.LengthShort);
                snackBar.Show();
            }
        }
    }
}