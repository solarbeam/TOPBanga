﻿using Android.Content;
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
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
                    .Build();
            googleApiClient.RegisterConnectionCallbacks(new SignInResultCallback(googleApiClient));
            _context = context;
        }

        public static LoginManager GetInstance(Context context) 
        {
            Instance = Instance ?? new LoginManager(context);
            return Instance;
        }

        public Intent SignInIntent => Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);

        public void OnResult(Java.Lang.Object result)
        {
            var googleSignInResult = result as GoogleSignInResult;
            HandleSignInResult(googleSignInResult);
        }

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
                userData.PutString(_context.GetString(Resource.String.google_id_key), result.SignInAccount.Id);
                userData.PutString(_context.GetString(Resource.String.google_id_name), result.SignInAccount.DisplayName);

                ((LoginActivity)_context).LoadMainMenu(userData);
            }
        }
    }
}
