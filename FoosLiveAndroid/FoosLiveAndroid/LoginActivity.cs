using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Common;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Util.Login;

namespace FoosLiveAndroid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]   
    public class LoginActivity : AppCompatActivity
    {
        static readonly string Tag = typeof(LoginActivity).Name;
        LoginManager _loginManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            SignInButton signInButton = FindViewById<SignInButton>(Resource.Id.sign_in_button);
            signInButton.Click += delegate {
                StartActivityForResult(_loginManager.SignInIntent, (int)RequestId.SignIn);
            };
            signInButton.SetSize(SignInButton.SizeStandard);
        }

        protected override void OnStart()
        {
            base.OnStart();
            // Initialize login handler
            _loginManager = LoginManager.GetInstance(this);
            //saved_auto_login
            //_loginManager.LoginSilent();
        }
        protected override void OnPause()
        {
            Log.Debug("LOCMAN", $"Connected: {_loginManager.googleApiClient.IsConnected}");
            base.OnPause();
            Log.Debug("LOCMAN", $"Connected: {_loginManager.googleApiClient.IsConnected}");
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == (int)RequestId.SignIn)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                _loginManager.HandleSignInResult(result);

            }
        }

        internal void LoadMainMenu(Bundle userData)
        {
            var intent = new Intent(Application.Context, typeof(MenuActivity));
            intent.PutExtra("USER_DATA", userData);
            StartActivity(intent);
        }
    }
}