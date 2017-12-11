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
            Task startupLoading = new Task(LoadResources);
            startupLoading.Start();
        }

        /// Background work behind the splash screen
        async void LoadResources()
        {
            // Initialize config file manager
            PropertiesManager.Initialise(this);

            // Initialize EmguCv
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

            // Check if the shared preference file exists
            var preferences = Application.Context.GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);
            if ( !( preferences.Contains("team1Score") && preferences.Contains("team1Win") &&
                preferences.Contains("team2Score") && preferences.Contains("team2Win") && preferences.Contains("soundEnabled")
                && preferences.Contains("syncEnabled") ) )
            {
                // It doesnt exist, so assign default values
                var prefsEditor = preferences.Edit();
                prefsEditor.PutString("team1Score", "defaultMarioGoal").Apply();
                prefsEditor.PutString("team1Win", "defaultMarioWin").Apply();
                prefsEditor.PutString("team2Score", "defaultMarioGoal").Apply();
                prefsEditor.PutString("team2Win", "defaultMarioWin").Apply();
                prefsEditor.PutBoolean("soundEnabled", true).Apply();
                prefsEditor.PutBoolean("syncEnabled", true).Apply();
                prefsEditor.Commit();
                prefsEditor.Dispose();
            }
            preferences.Dispose();

            // Start application
            StartActivity(new Intent(Application.Context, typeof(MenuActivity)));
        }
    }
}
