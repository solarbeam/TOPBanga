using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Java.Lang;

namespace FoosLiveAndroid.Util.Login
{
    public class SignInResultCallback : Object, IResultCallback
	{
		public LoginActivity Activity { get; set; }

		public void OnResult(Object result)
		{
			var googleSignInResult = result as GoogleSignInResult;
			Activity.HideProgressDialog();
			Activity.HandleSignInResult(googleSignInResult);
		}
	}
}
