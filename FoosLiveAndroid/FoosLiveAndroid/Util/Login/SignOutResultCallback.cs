using Android.Gms.Common.Apis;
using Java.Lang;

namespace FoosLiveAndroid.Util.Login
{
	public class SignOutResultCallback : Object, IResultCallback
	{
		public LoginActivity Activity { get; set; }

		public void OnResult(Object result)
		{
			Activity.UpdateUI(false);
		}
	}
}
