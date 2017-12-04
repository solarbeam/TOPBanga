using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.V7.App;

namespace FoosLiveAndroid
{
    [Activity(Theme = "@style/FoosbalTheme.Splash", MainLauncher = true, NoHistory = true, Label = "Fooslive", Icon = "@mipmap/icon_round")]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = typeof(SplashActivity).Name;

        /// Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(LoadResources);
            startupWork.Start();
        }

        /// Background work behind the splash screen
        async void LoadResources()
        {
            // Todo: initialise resources here

            await Task.Delay(1000);// simulate loading

            StartActivity(new Intent(Application.Context, typeof(MenuActivity)));
        }
    }
}
