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

        private const string preferencesFileName = "FoosliveAndroid.dat";

        private const string team1Score = "team1Score";
        private const string team1ScoreDefault = "defaultMarioGoal";

        private const string team1Win = "team1Win";
        private const string team1WinDefault = "defaultMarioWin";

        private const string team2Score = "team2Score";
        private const string team2Default = "defaultMarioGoal";

        private const string team2Win = "team2Win";
        private const string team2WinDefault = "defaultMarioWin";

        private const string soundEnabled = "soundEnabled";
        private const bool soundEnabledDefault = true;

        private const string syncEnabled = "syncEnabled";
        private const bool syncEnabledDefault = true;

        private const string team1Name = "team1Name";
        private const string team1NameDefault = "Team 1";
        private const string team2Name = "team2Name";
        private const string team2NameDefault = "Team 2";

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

            // Todo: move to for loop
            // Check if the shared preference file exists
            var preferences = Application.Context.GetSharedPreferences(preferencesFileName, FileCreationMode.Private);
            if ( !( preferences.Contains(team1Score) && preferences.Contains(team1Win) &&
                preferences.Contains(team2Score) && preferences.Contains(team2Win) && preferences.Contains(soundEnabled)
                   && preferences.Contains(syncEnabled) &&  preferences.Contains(team1Name) && preferences.Contains(team2Name) ) )
            {
                // It doesnt exist, so assign default values
                var prefsEditor = preferences.Edit();
                prefsEditor.PutString(team1Score, team1ScoreDefault).Apply();
                prefsEditor.PutString(team1Win, team1WinDefault).Apply();
                prefsEditor.PutString(team2Score, team2WinDefault).Apply();
                prefsEditor.PutString(team2Win, team2WinDefault).Apply();
                prefsEditor.PutBoolean(soundEnabled, soundEnabledDefault).Apply();
                prefsEditor.PutBoolean(syncEnabled, syncEnabledDefault).Apply();
                prefsEditor.PutString(team1Name, team1NameDefault).Apply();
                prefsEditor.PutString(team2Name, team2NameDefault).Apply();
                prefsEditor.Commit();
                prefsEditor.Dispose();
            }
            preferences.Dispose();

            // Start application
            StartActivity(new Intent(Application.Context, typeof(MenuActivity)));
        }
    }
}
