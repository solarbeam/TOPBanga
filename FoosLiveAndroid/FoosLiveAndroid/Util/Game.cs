using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Util.GameControl;
using FoosLiveAndroid.Util.Detection;
using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Util.Sounds;
using Android.Content.Res;
using System.Threading.Tasks;
using FoosLiveAndroid.Util.Model;
using FoosLiveAndroid.Util.Drawing;
using Android.Util;

namespace FoosLiveAndroid.Util
{
    class Game
    {
        static readonly string Tag = typeof(GameActivity).Name;
        private static readonly float FormatSpeed = PropertiesManager.GetFloatProperty("format_speed");
        private static readonly int TimerFrequency = PropertiesManager.GetIntProperty("timer_frequency");

        private bool _textThreadStarted = false;
        private bool _waitForSpeed = false;
        
        // Controlled UI elements
        private Activity activity;
        private TextView _ballSpeed;
        private TextView _score;
        private TextView _eventText;
        private TextView _timer;

        public GameController _gameController;
        public IColorDetector _colorDetector;
        public IObjectDetector _objectDetector;
        public GameTimer _gameTimer;

        private SoundAlerts _soundAlerts;

        public Game(float mulX, float mulY, Activity activity, TextView ballSpeed, TextView score, TextView eventText, TextView timer)
        {
            _colorDetector = new ColorDetector();
            _gameController = new GameController();
            _gameTimer = new GameTimer(TimerFrequency);
            _objectDetector = new ObjectDetector(mulX, mulY, _colorDetector, _gameController);
            _gameTimer.OnUpdated += UpdateTimer;
            _gameController.GoalEvent += GameControllerGoalEvent;
            _gameController.PositionEvent += GameControllerPositionEvent;

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

                var team1WinDefaultValue = activity.Resources.GetString(Resource.String.saved_team1_win_default);
                var team2WinDefaultValue = activity.Resources.GetString(Resource.String.saved_team2_win_default);
                var team1GoalDefaultValue = activity.Resources.GetString(Resource.String.saved_team1_goal_default);
                var team2GoalDefaultValue = activity.Resources.GetString(Resource.String.saved_team2_goal_default);

                _soundAlerts = new SoundAlerts
                {
                    Team1Win = new PlayerOGG(FilePathResolver.GetFile(activity, preferences.GetString(team1WinKey, team1WinDefaultValue))),
                    Team1Goal = new PlayerOGG(FilePathResolver.GetFile(activity, preferences.GetString(team1GoalKey, team1GoalDefaultValue))),
                    Team2Win = new PlayerOGG(FilePathResolver.GetFile(activity, preferences.GetString(team2WinKey, team2WinDefaultValue))),
                    Team2Goal = new PlayerOGG(FilePathResolver.GetFile(activity, preferences.GetString(team2GoalKey, team2GoalDefaultValue)))
                };
            }

            this.activity = activity;
        }

        private void UpdateTimer(object sender, EventArgs e)
        {
            activity.RunOnUiThread(() =>
            {
                _timer.Text = _gameTimer.GetFormattedTime();
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
            if (!_textThreadStarted && !_waitForSpeed)
            {
                if (_gameController.CurrentSpeed >= FormatSpeed)
                {
                    _ballSpeed.Text = Math.Round(_gameController.CurrentSpeed, 1).ToString();
                }
                else
                    _ballSpeed.Text = Math.Round(_gameController.CurrentSpeed, 2).ToString();

                _waitForSpeed = true;

                // Delay the new speed information
                activity.RunOnUiThread(async () =>
                {
                    await Task.Delay(80);
                    _waitForSpeed = false;
                });
            }
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
                _soundAlerts?.Play(EAlert.Team1Goal);
                TextEffects.SlideText(activity.ApplicationContext.Resources.GetString(Resource.String.blue_team_goal), activity, _eventText);
            }
            else if (e == CurrentEvent.RedGoalOccured)
            {
                _soundAlerts?.Play(EAlert.Team2Goal);
                TextEffects.SlideText(activity.ApplicationContext.Resources.GetString(Resource.String.red_team_goal), activity, _eventText);
            }
            _score.Text = $"{_gameController.BlueScore} : {_gameController.RedScore}";
            Log.Debug(Tag, $"Score value assigned {_score.Text}");
        }
    }
}