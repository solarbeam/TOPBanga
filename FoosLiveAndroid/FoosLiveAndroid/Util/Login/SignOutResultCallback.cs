using Android.Content;
using Android.Gms.Common.Apis;
using Java.Lang;

namespace FoosLiveAndroid.Util.Login
{
	public class SignOutResultCallback : Object, IResultCallback
	{
        private GoogleApiClient _client;
        public SignOutResultCallback(GoogleApiClient googleApiClient)
        {
            _client = googleApiClient;
        }

		public void OnResult(Object result)
		{
            _client.Disconnect();
		}
	}
}
