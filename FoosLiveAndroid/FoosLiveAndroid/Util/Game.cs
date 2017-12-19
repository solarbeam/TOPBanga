using System;
using Android.App;
using Android.Content;
using Android.Widget;
using FoosLiveAndroid.Util.GameControl;
using FoosLiveAndroid.Util.Detection;
using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Util.Sounds;
using System.Threading.Tasks;
using FoosLiveAndroid.Util.Model;
using FoosLiveAndroid.Util.Drawing;
using Android.Util;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Util
{
    class Game
    {
        static readonly string Tag = typeof(GameActivity).Name;
        private static readonly float FormatSpeed = PropertiesManager.GetFloatProperty("format_speed");
        private static readonly int TimerFrequency = PropertiesManager.GetIntProperty("timer_frequency");

        private readonly bool _textThreadStarted = false;
        private bool _waitForSpeed = false;
        private readonly string _scoreFormat;
        private readonly int MaxCharLength;

        private string team1name;
        private string team2name;

        // Controlled UI elements
        private Activity _activity;
        private TextView _ballSpeed;
        private TextView _score;
        private TextView _eventText;
        private TextView _timer;

        public GameController GameController;
        public IColorDetector ColorDetector;
        public ObjectDetector ObjectDetector;
        public GameTimer GameTimer;

        public SoundAlerts SoundAlerts;

        public Game(float mulX, float mulY, Activity activity, TextView ballSpeed, TextView score, TextView eventText, TextView timer)
        {
            ColorDetector = new ColorDetector();
            GameController = new GameController();
            GameTimer = new GameTimer(TimerFrequency);
            ObjectDetector = new ObjectDetector(mulX, mulY, ColorDetector, GameController);
            GameController.GoalEvent += GameControllerGoalEvent;
            GameController.PositionEvent += GameControllerPositionEvent;
            GameTimer.OnUpdated += UpdateTimer;

            MaxCharLength = int.Parse(activity.GetString(Resource.Integer.max_chars));

            _ballSpeed = ballSpeed;
            _score = score;
            _eventText = eventText;
            _timer = timer;

            // Assign the sound file paths
            var preferences = activity.GetSharedPreferences(activity.GetString(Resource.String.preference_file_key), FileCreationMode.Private);

            var defaultSoundEnabledValue = activity.Resources.GetBoolean(Resource.Boolean.saved_sound_enabled_default);
            if (preferences.GetBoolean(activity.GetString(Resource.String.saved_sound_enabled), defaultSoundEnabledValue))
            {
                var team1WinKey = activity.Resources.GetString(Resource.String.saved_team1_win);
                var team2WinKey = activity.Resources.GetString(Resource.String.saved_team2_win);
                var team1GoalKey = activity.Resources.GetString(Resource.String.saved_team1_goal);
                var team2GoalKey = activity.Resources.GetString(Resource.String.saved_team2_goal);

                // Todo: use string resources instead of enum

                SoundAlerts = new SoundAlerts
                {
                    Team1Win = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team1WinKey, SoundAsset.WinMario.ToString()))),
                    Team1Goal = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team1GoalKey, SoundAsset.GoalMario.ToString()))),
                    Team2Win = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team2WinKey, SoundAsset.WinMario.ToString()))),
                    Team2Goal = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team2GoalKey, SoundAsset.GoalMario.ToString())))
                };
            }

            team1name = preferences.GetString(activity.GetString(Resource.String.saved_team1_name), activity.GetString(Resource.String.team1));
            team2name = preferences.GetString(activity.GetString(Resource.String.saved_team2_name), activity.GetString(Resource.String.team2));

            preferences.Dispose();

            _scoreFormat = activity.GetString(Resource.String.score_format);
            _activity = activity;
        }

        private void UpdateTimer(object sender, EventArgs e)
        {
            _activity.RunOnUiThread(() =>
            {
                _timer.Text = GameTimer.GetFormattedTime();
            });
        }

        /// <summary>
        /// Fired whenever the ball's position changes
        /// </summary>
        /// <param name="sender">The class, which called this function</param>
        /// <param name="e">Arguments, which are passed to this function</param>
        private void GameControllerPositionEvent(object sender, EventArgs e)
        {
            // Check if sliding text is active or the delay is still on
            if (_textThreadStarted || _waitForSpeed) return;
            if (GameController.CurrentSpeed >= FormatSpeed)
            {
                _ballSpeed.Text = Math.Round(GameController.CurrentSpeed, 1).ToString();
            }
            else
                _ballSpeed.Text = Math.Round(GameController.CurrentSpeed, 2).ToString();

            _waitForSpeed = true;

            // Delay the new speed information
            _activity.RunOnUiThread(async () =>
            {
                await Task.Delay(80);
                _waitForSpeed = false;
            });
        }

        /// <summary>
        /// Called whenever a goal event occurs
        /// </summary>
        /// <param name="sender">The class, which called this function</param>
        /// <param name="e">Arguments, which are passed to this function</param>
        private void GameControllerGoalEvent(object sender, CurrentEvent e)
        {
            // Check which event occured
            if (e == CurrentEvent.BlueGoalOccured)
            {
                SoundAlerts?.Play(EAlert.Team1Goal);
                TextEffects.SlideText(team1name + " scored!", _activity, _eventText,MaxCharLength);
            }
            else if (e == CurrentEvent.RedGoalOccured)
            {
                SoundAlerts?.Play(EAlert.Team2Goal);
                TextEffects.SlideText(team2name + " scored!", _activity, _eventText, MaxCharLength);
            }
            _score.Text = string.Format(_scoreFormat, GameController.BlueScore, GameController.RedScore);
            Log.Debug(Tag, $"Score value assigned {_score.Text}");

            // Reset the speed counter
            _activity.RunOnUiThread(() => _ballSpeed.Text = "0");
        }
    }
}