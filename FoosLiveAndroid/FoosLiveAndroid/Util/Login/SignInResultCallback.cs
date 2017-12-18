using Android.Content;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Util;
using Java.Lang;
using static Android.Gms.Common.Apis.GoogleApiClient;

namespace FoosLiveAndroid.Util.Login
{
    public class SignInResultCallback : Object, IResultCallback, IConnectionCallbacks
    {
        private GoogleApiClient _client;
        public SignInResultCallback(GoogleApiClient googleApiClient)
        {
            _client = googleApiClient;
        }
        public Context LoginContext { get; set; }

        public void OnConnected(Bundle connectionHint)
        {
            _client.ClearDefaultAccountAndReconnect();
        }

        public void OnConnectionSuspended(int cause) {}

        public void OnResult(Object result) {}
    }
}
