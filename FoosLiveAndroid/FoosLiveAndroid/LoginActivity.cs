using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Util.Login;

namespace FoosLiveAndroid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]   
    public class LoginActivity : AppCompatActivity, View.IOnClickListener, GoogleApiClient.IOnConnectionFailedListener
    {
        static readonly string TAG = typeof(LoginActivity).Name;

        GoogleApiClient mGoogleApiClient;
        ProgressDialog mProgressDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            FindViewById(Resource.Id.sign_in_button).SetOnClickListener(this);

            // [START configure_signin]
            // Configure sign-in to request the user's ID, email address, and basic
            // profile. ID and basic profile are included in DEFAULT_SIGN_IN.
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail()
                    .Build();
            // [END configure_signin]

            // [START build_client]
            // Build a GoogleApiClient with access to the Google Sign-In API and the
            // options specified by gso.
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this /* FragmentActivity */, this /* OnConnectionFailedListener */)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();
            // [END build_client]

            // [START customize_button]
            // Set the dimensions of the sign-in button.
            var signInButton = FindViewById<SignInButton>(Resource.Id.sign_in_button);
            signInButton.SetSize(SignInButton.SizeStandard);
            // [END customize_button]
        }

        protected override void OnStart()
        {
            base.OnStart();

            var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            if (opr.IsDone)
            {
                // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
                // and the GoogleSignInResult will be available instantly.
                Log.Debug(TAG, "Got cached sign-in");
                var result = opr.Get() as GoogleSignInResult;
                HandleSignInResult(result);
            }
            else
            {
                // If the user has not previously signed in on this device or the sign-in has expired,
                // this asynchronous branch will attempt to sign in the user silently.  Cross-device
                // single sign-on will occur in this branch.
                ShowProgressDialog();
                opr.SetResultCallback(new SignInResultCallback { Activity = this });
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            HideProgressDialog();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Log.Debug(TAG, "onActivityResult:" + requestCode + ":" + resultCode + ":" + data);

            // Result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == (int)RequestId.SignIn)
            {
                var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                HandleSignInResult(result);
            }
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            Log.Debug(TAG, "handleSignInResult:" + result.IsSuccess);
            if (result.IsSuccess)
            {
                // Signed in successfully, show authenticated UI.
                var acct = result.SignInAccount;
                StartActivity(new Intent(Application.Context, typeof(MenuActivity)));
            }
            else
            {
                // Signed out, show unauthenticated UI.
                Toast.MakeText(this, "Error", ToastLength.Long);
                Log.Debug(TAG, $"Status: {result.Status} , peer ref-{result.PeerReference}");
            }
        }

        void SignIn()
        {
            var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, (int)RequestId.SignIn);
        }

        void SignOut()
        {
            Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
        }

        void RevokeAccess()
        {
            Auth.GoogleSignInApi.RevokeAccess(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            // An unresolvable error has occurred and Google APIs (including Sign-In) will not
            // be available.
            Log.Debug(TAG, "onConnectionFailed:" + result);
        }

        protected override void OnStop()
        {
            base.OnStop();
            mGoogleApiClient.Disconnect();
        }

        public void ShowProgressDialog()
        {
            if (mProgressDialog == null)
            {
                mProgressDialog = new ProgressDialog(this);
                mProgressDialog.SetMessage("Loading");
                mProgressDialog.Indeterminate = true;
            }

            mProgressDialog.Show();
        }

        public void HideProgressDialog()
        {
            if (mProgressDialog != null && mProgressDialog.IsShowing)
            {
                mProgressDialog.Hide();
            }
        }

        public void UpdateUI(bool isSignedIn)
        {
            if (isSignedIn)
            {
            }
            else
            {
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.sign_in_button:
                    SignIn();
                    break;
            }
        }
    }
}