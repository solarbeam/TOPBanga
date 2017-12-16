﻿using System;
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

namespace FoosLiveAndroid.Util
{
    class Game
    {
        static readonly string Tag = typeof(GameActivity).Name;
        private static readonly float FormatSpeed = PropertiesManager.GetFloatProperty("format_speed");
        private static readonly int TimerFrequency = PropertiesManager.GetIntProperty("timer_frequency");

        private readonly bool _textThreadStarted = false;
        private bool _waitForSpeed = false;
        
        // Controlled UI elements
        private Activity _activity;
        private TextView _ballSpeed;
        private TextView _score;
        private TextView _eventText;
        private TextView _timer;

        public GameController GameController;
        public IColorDetector ColorDetector;
        public IObjectDetector ObjectDetector;
        public GameTimer GameTimer;

        private SoundAlerts _soundAlerts;

        public Game(float mulX, float mulY, Activity activity, TextView ballSpeed, TextView score, TextView eventText, TextView timer)
        {
            ColorDetector = new ColorDetector();
            GameController = new GameController();
            GameTimer = new GameTimer(TimerFrequency, activity.Resources);
            ObjectDetector = new ObjectDetector(mulX, mulY, ColorDetector, GameController);
            GameController.GoalEvent += GameControllerGoalEvent;
            GameController.PositionEvent += GameControllerPositionEvent;

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
                    Team1Win = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team1WinKey, team1WinDefaultValue))),
                    Team1Goal = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team1GoalKey, team1GoalDefaultValue))),
                    Team2Win = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team2WinKey, team2WinDefaultValue))),
                    Team2Goal = new PlayerOgg(FilePathResolver.GetFile(activity, preferences.GetString(team2GoalKey, team2GoalDefaultValue)))
                };
            }

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
                _soundAlerts?.Play(EAlert.Team1Goal);
                TextEffects.SlideText(_activity.ApplicationContext.Resources.GetString(Resource.String.blue_team_goal), _activity, _eventText);
            }
            else if (e == CurrentEvent.RedGoalOccured)
            {
                _soundAlerts?.Play(EAlert.Team2Goal);
                TextEffects.SlideText(_activity.ApplicationContext.Resources.GetString(Resource.String.red_team_goal), _activity, _eventText);
            }
            _score.Text = $"{GameController.BlueScore} : {GameController.RedScore}";
            Log.Debug(Tag, $"Score value assigned {_score.Text}");
        }
    }
}