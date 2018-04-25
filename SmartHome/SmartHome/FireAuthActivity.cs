using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using static Android.Views.View;
using Android.Gms.Tasks;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Text;

using Firebase.Auth;


namespace SmartHome
{
    [Activity(Label = "FireAuthActivity")]
    public class FireAuthActivity : AppCompatActivity, IOnClickListener, IOnCompleteListener
    {
        Button btnSignup;
        TextView btnLogin, btnForgotPass;
        EditText input_email, input_password, input_password_confirm;
        RelativeLayout activity_sign_up;
        LinearLayout linL;

        FirebaseAuth auth;

        public delegate void onReadyToWorkWithDBA(FirebaseAuth a);
        public static event onReadyToWorkWithDBA readyA;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);

            auth = FireCfg.auth;

            
            btnSignup = FindViewById<Button>(Resource.Id.signup_btn_register);
            btnLogin = FindViewById<TextView>(Resource.Id.signup_btn_login);
            btnForgotPass = FindViewById<TextView>(Resource.Id.signup_btn_forgot_password);
            input_email = FindViewById<EditText>(Resource.Id.signup_email);
            input_password = FindViewById<EditText>(Resource.Id.signup_password);
            input_password_confirm = FindViewById<EditText>(Resource.Id.signup_password_confirm);
            activity_sign_up = FindViewById<RelativeLayout>(Resource.Id.activity_sign_up);
            linL = FindViewById<LinearLayout>(Resource.Id.signup_layout_or);

            btnLogin.SetOnClickListener(this);
            btnForgotPass.SetOnClickListener(this);
            btnSignup.SetOnClickListener(this);

        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.signup_btn_login)
            {
                linL.Visibility = ViewStates.Gone;
                btnLogin.Visibility = ViewStates.Gone;
                input_password_confirm.Visibility = ViewStates.Gone;
                btnSignup.Text = "Login";
                btnSignup.Click += OnClickListenerForLogin;
            }
            else if (v.Id == Resource.Id.signup_btn_forgot_password)
            {
                StartActivity(new Intent(this, typeof(ForgotPassword)));
               //Finish();
            }
            else if (v.Id == Resource.Id.signup_btn_register)
            {
                SignUpUser(input_email.Text, input_password.Text, input_password_confirm.Text);
            }
        }
        void OnClickListenerForLogin(object o, EventArgs e)
        {
            LoginUser(input_email.Text, input_password.Text);
        }

        private void LoginUser(string email, string password)
        {
            auth.SignInWithEmailAndPassword(email, password)
                .AddOnCompleteListener(this);
        }

        private void SignUpUser(string email, string password, string passwordConfirm)
        {
            if (IsValidEmailAndPassword(email, password))
            {
                if (password.Equals(passwordConfirm))
                {
                    auth.CreateUserWithEmailAndPassword(email, password)
                        .AddOnCompleteListener(this, this);
                }
                else input_password_confirm.SetError("Passwords do not match", GetDrawable(Resource.Drawable.ic_error_white_18dp));
            }
            else input_email.SetError("Invalid email", GetDrawable(Resource.Drawable.ic_error_white_18dp));
        }

        private bool IsValidEmailAndPassword(string email, string password)
        {
            return !TextUtils.IsEmpty(email) && Patterns.EmailAddress.Matcher(email).Matches() && !email.Contains(' ') && !TextUtils.IsEmpty(password) && !password.Contains(' ');
        }

        public void OnComplete(Task task)
        {
            if (task.IsSuccessful == true)
            {
                Snackbar snackBar = Snackbar.Make(activity_sign_up, "Successfully", Snackbar.LengthShort);
                snackBar.Show();
                Log.Debug("USER", "" + auth.CurrentUser);
                StartActivity(new Intent(this, typeof(MainActivity)));
                if (auth.CurrentUser != null)
                {
                    Log.Debug("FireAuth", "auth = " + auth.CurrentUser);
                    readyA(auth);
                    ISharedPreferencesEditor sharedEditor = FireCfg.sharedPref.Edit();
                    sharedEditor.PutString("user_email", auth.CurrentUser.Email);
                    sharedEditor.Apply();
                }
            }
            else
            {
                Snackbar snackBar = Snackbar.Make(activity_sign_up, "Failed ", Snackbar.LengthShort);
                snackBar.Show();
            }
        }
    }

  
}