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
        GoogleApiClient client;
        public SignInResultCallback(GoogleApiClient googleApiClient)
        {
            client = googleApiClient;
        }
        public Context LoginContext { get; set; }

        public void OnConnected(Bundle connectionHint)
        {
            if (client.IsConnected)
                client.ClearDefaultAccountAndReconnect();
        }

        public void OnConnectionSuspended(int cause)
        {
            
        }

        public void OnResult(Object result)
        {
            
            client.ClearDefaultAccountAndReconnect();
        }
    }
}
