using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;
using FoosLiveAndroid.Util;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Cvb;

namespace FoosLiveAndroid
{
    [Activity(Theme = "@style/FoosbalTheme.Splash", MainLauncher = true,
              NoHistory = true, Label = "Fooslive", Icon = "@mipmap/icon_round",
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = typeof(SplashActivity).Name;
        private static int Iterations = 1;

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
            PropertiesManager.Initialise(this);

            // Initialize EmguCv. Trust me, it works
            var tempImage = new Image<Bgr, byte>(200, 200, new Bgr(255, 0, 100));
            var filtered = tempImage.InRange(new Bgr(100, 0, 0), new Bgr(200, 0, 0));
            filtered.Erode(Iterations).Dispose();
            filtered.Dilate(Iterations).Dispose();
            CvBlobDetector tempDetector = new CvBlobDetector();
            CvBlobs blobs = new CvBlobs();
            tempDetector.Detect(filtered, blobs);

            tempImage.Dispose();
            filtered.Dispose();
            blobs.Dispose();
            tempDetector.Dispose();
            // Initialization end

            StartActivity(new Intent(Application.Context, typeof(MenuActivity)));
        }
    }
}
