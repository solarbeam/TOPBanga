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
              NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = typeof(SplashActivity).Name;
        private static int Iterations = 1;

        /// Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            // Initialize loading
            new Task(LoadResources).Start();
        }

        /// Background work behind the splash screen
        private void LoadResources()
        {
            // Initialize config file manager
            PropertiesManager.Initialise(this);

            // Initialize EmguCv
            var tempImage = new Image<Bgr, byte>(200, 200, new Bgr(255, 0, 100));
            var filtered = tempImage.InRange(new Bgr(100, 0, 0), new Bgr(200, 0, 0));
            filtered.Erode(Iterations).Dispose();
            filtered.Dilate(Iterations).Dispose();
            var tempDetector = new CvBlobDetector();
            var blobs = new CvBlobs();
            tempDetector.Detect(filtered, blobs);

            tempImage.Dispose();
            filtered.Dispose();
            blobs.Dispose();
            tempDetector.Dispose();

            // Start application
            StartActivity(new Intent(Application.Context, typeof(LoginActivity)));
        }
    }
}
