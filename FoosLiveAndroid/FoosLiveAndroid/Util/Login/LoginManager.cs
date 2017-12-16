using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Util;

namespace FoosLiveAndroid.Util.Login
{
    public class LoginManager : Java.Lang.Object, GoogleApiClient.IOnConnectionFailedListener
    {
        static readonly string Tag = typeof(LoginManager).Name;
        static LoginManager Instance;
        public GoogleApiClient googleApiClient;
        public bool disconnect = false;
        private Context _context;

        private LoginManager(Context context)
        {
            // Configure sign-in to request the user's ID, email address, and basic
            // profile. ID and basic profile are included in DEFAULT_SIGN_IN.
            var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                                                       .RequestEmail().Build();
            // Build a GoogleApiClient with access to the Google Sign-In API and the
            // options specified by gso.
            googleApiClient = new GoogleApiClient.Builder(context)
                    .EnableAutoManage((LoginActivity)context, OnResult)
                                                 .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions).AddConnectionCallbacks(new SignInResultCallback(googleApiClient))
                    .Build();
            _context = context;
        }

        public static LoginManager GetInstance(Context context) 
        {
            Instance = Instance ?? new LoginManager(context);
            return Instance;
        }

        public Intent SignInIntent 
        {
            get 
            {
                if (googleApiClient.HasConnectedApi(Auth.GOOGLE_SIGN_IN_API))
                {
                    googleApiClient.ClearDefaultAccountAndReconnect();
                }
                return Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
            }
        }

        public void OnResult(Java.Lang.Object result)
        {
            var googleSignInResult = result as GoogleSignInResult;
            HandleSignInResult(googleSignInResult);
        }

        //public void LoginSilent() 
        //{
        //    if (disconnect) 
        //    {
        //        disconnect = false;
        //        return;
        //    }
        //    var optionalPendingResult = Auth.GoogleSignInApi.SilentSignIn(googleApiClient);
        //    if (optionalPendingResult.IsDone)
        //    {
        //        // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
        //        // and the GoogleSignInResult will be available instantly.
        //        Log.Debug(Tag, "Got cached sign-in");
        //        var result = optionalPendingResult.Get() as GoogleSignInResult;
        //        HandleSignInResult(result);
        //        return;
        //    }
        //    // If the user has not previously signed in on this device or the sign-in has expired,
        //    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
        //    // single sign-on will occur in this branch.
        //    optionalPendingResult.SetResultCallback(new SignInResultCallback(googleApiClient));
        //}

        public void OnConnectionFailed(ConnectionResult result)
        {
            // An unresolvable error has occurred and Google APIs (including Sign-In) will not
            // be available.
            Log.Debug(Tag, "onConnectionFailed:" + result);
        }

        public void HandleSignInResult(GoogleSignInResult result)
        {
            Log.Debug(Tag, "handleSignInResult:" + result.IsSuccess);
            if (result.IsSuccess)
            {
                // Signed in successfully
                var userData = new Bundle();
                // Transfer result data to MenuActivity
                userData.PutString("ID", result.SignInAccount.Id);
                userData.PutString("NAME", result.SignInAccount.DisplayName);

                ((LoginActivity)_context).LoadMainMenu(userData);
            }
            else
            {
                //Toast.MakeText(_context, result.Status.StatusMessage, ToastLength.Long).Show();
            }
        }

        public void SignOut()
        {
            disconnect = true;
            Log.Debug("LOCMAN", $"Connected: {googleApiClient.IsConnected}");
            if (googleApiClient.HasConnectedApi(Auth.GOOGLE_SIGN_IN_API))
            {
                googleApiClient.ClearDefaultAccountAndReconnect();
            }

            if (googleApiClient != null && googleApiClient.IsConnected)
            {
                googleApiClient.ClearDefaultAccountAndReconnect().SetResultCallback(new SignOutResultCallback(googleApiClient));
            }
        }



        void RevokeAccess()
        {
            Auth.GoogleSignInApi.RevokeAccess(googleApiClient).SetResultCallback(new SignOutResultCallback(googleApiClient));
        }
    }
}
