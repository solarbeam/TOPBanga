﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Emgu.CV;
using Camera = Android.Hardware.Camera;
using Emgu.CV.Structure;
using Android.Util;
using FoosLiveAndroid.Util;
using System;
using Android.Hardware;
using FoosLiveAndroid.Util.Drawing;
using Android.Content;
using FoosLiveAndroid.Util.Sensors;
using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Fragments;
using FoosLiveAndroid.Util.Model;
using FoosLiveAndroid.Model;
using Android.Support.V7.App;
using FoosLiveAndroid.Util.Record;
using FoosLiveAndroid.Util.Database;
using System.Threading.Tasks;

namespace FoosLiveAndroid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : AppCompatActivity, View.IOnTouchListener
    {
        static readonly string Tag = typeof(GameActivity).Name;
        private static readonly int CameraWidth = PropertiesManager.GetIntProperty("camera_width");
        private static readonly int CameraHeight = PropertiesManager.GetIntProperty("camera_height");

        private static readonly int PreviewWidth = PropertiesManager.GetIntProperty("preview_width");
        private static readonly int PreviewHeight = PropertiesManager.GetIntProperty("preview_height");

        private static readonly int ColorZoneCount = PropertiesManager.GetIntProperty("color_zone_count");

        // A constant for upscaling the positions
        private float _upscaleMultiplierX;
        private float _upscaleMultiplierY;

        private TextView _eventText;
        private TextView _timer;
        private Button _gameButton;
        private TextView _score;
        public TextureView GameView;
        private SurfaceView _surfaceView;
        private TextView _ballSpeed;
        private TextView _team1Title;
        private TextView _team2Title;
        private Button _addScoreTeam1;
        private Button _addScoreTeam2;
        private Button _removeScoreTeam1;
        private Button _removeScoreTeam2;
            
        private string _scoreFormat;

        // Guideline UI elements
        private ImageView _arrowTop;
        private ImageView _arrowLeft;
        private ImageView _arrowRight;
        private ImageView _arrowBot;

        private FrameLayout _infoLayout;
        private RelativeLayout _topBar;

        private Game _game;

        public ECaptureMode GameMode;
        private bool _gameEnd;

        // Todo: change Camera to Camera2
        private Camera _camera;

        internal SurfaceManager SurfaceManager;

        private Hsv _selectedBallColor;
        internal bool BallColorSelected;
        private Image<Hsv, byte> _image;

        private IPositionManager _positionManager;
        private RecordPlayer _recordPlayer;

        /// <summary>
        /// Called whenever the view is created
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Identify game capture mode
            if (Intent.Data != null)
            {
                GameMode = ECaptureMode.Recording;
            }
            else
            {
                GameMode = ECaptureMode.Live;
                // Set up sensors & vibration
                var sensorManager = (SensorManager)GetSystemService(SensorService);
                IVibration vibration = new Vibration((Vibrator)GetSystemService(VibratorService));
                _positionManager = new PositionManager(this, sensorManager, vibration);
            }

            SetContentView(Resource.Layout.activity_game);

            //Hide notification bar
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            //Prevent from sleeping
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            GetReferencesFromLayout();

            SurfaceManager = new SurfaceManager(this, _surfaceView.Holder);
            
            _surfaceView.SetZOrderOnTop(true);
            _surfaceView.Holder.SetFormat(Format.Transparent);

            _gameButton.Click += GameButtonClicked;

            _addScoreTeam1.Click += (o, e) =>
            {
                _game.GameController.BlueScore++;
                _score.Text = string.Format(_scoreFormat, _game.GameController.BlueScore, _game.GameController.RedScore);
            };
            _addScoreTeam2.Click += (o, e) =>
            {
                _game.GameController.RedScore++;
                _score.Text = string.Format(_scoreFormat, _game.GameController.BlueScore, _game.GameController.RedScore);
            };
            _removeScoreTeam1.Click += (o, e) =>
            {
                if (_game.GameController.BlueScore == 0)
                    return;
                _game.GameController.BlueScore--;
                _score.Text = string.Format(_scoreFormat, _game.GameController.BlueScore, _game.GameController.RedScore);
            };
            _removeScoreTeam2.Click += (o, e) =>
            {
                if (_game.GameController.RedScore == 0)
                    return;
                _game.GameController.RedScore--;
                _score.Text = string.Format(_scoreFormat, _game.GameController.BlueScore, _game.GameController.RedScore);
            };

            // Assign the sound file paths
            var preferences = GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);

            var team1DefaultValue = Resources.GetString(Resource.String.saved_team1_name_default);
            _team1Title.Text = preferences.GetString(GetString(Resource.String.saved_team1_name), team1DefaultValue);

            var team2DefaultValue = Resources.GetString(Resource.String.saved_team2_name_default);
            _team2Title.Text = preferences.GetString(GetString(Resource.String.saved_team2_name), team2DefaultValue);

            preferences.Dispose();

            _scoreFormat = GetString(Resource.String.score_format);
            //_timerFormat = GetString(Resource.String.timer_format);

            // Open the camera
            GameView.SurfaceTextureListener = SurfaceManager;
            GameView.SetOnTouchListener(this);
            CvInvoke.UseOptimized = true;
        }

        internal void ReleaseResources()
        {
            // Check if we use a video file for getting frames or the camera
            if (GameMode == ECaptureMode.Recording)
            {
                // We use a video file, so release it's resources
                _recordPlayer.Release();
            }
            else
            {
                // We use a camera, so release it
                _camera?.Release();
                if (_camera == null)
                    Log.Error(Tag, "Camera call from OnSurfaceTextureDestroyed on null reference");
            }

            _positionManager?.StopListening();
        }

        public void SetMultipliers(float screenWidth, float screenHeight)
        {
            _upscaleMultiplierY = screenHeight / PreviewHeight;
            _upscaleMultiplierX = screenWidth / PreviewWidth;

            // Create the Game class instance
            _game = new Game(_upscaleMultiplierX, _upscaleMultiplierY, this, _ballSpeed, _score, _eventText, _timer);
        }

        internal void SetUpCameraMode()
        {
            // Draw the align zones
            var canvas = AlignZones.DrawZones(SurfaceManager.SurfaceHolder.LockCanvas(), _game.GameController);
            SurfaceManager.SurfaceHolder.UnlockCanvasAndPost(canvas);

            _camera = Camera.Open();

            // Get the camera parameters in order to set the appropriate frame size
            var parameters = _camera.GetParameters();
            var list = _camera.GetParameters().SupportedPreviewSizes;

            // Go through all of the sizes until we find an appropriate one
            foreach (var size in list)
            {
                if (size.Width > CameraWidth || size.Height > CameraHeight) continue;
                // The size matches or is lower than that of the constants camera_width, camera_height 
                parameters.SetPreviewSize(size.Width, size.Height);
                break;
            }

            _camera.SetParameters(parameters);
            _camera.SetDisplayOrientation(90);

            try
            {
                /**
                 * Set the surface on which frames are drawn
                 * In this case, it's the surface, which was created for _gameview
                 */
                _camera.SetPreviewTexture(SurfaceManager.SurfaceTexture);
                _camera.StartPreview();
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(Tag, e.Message);
                throw;
            }
        }

        internal void SetUpRecordMode(float screenWidth, float screenHeight)
        {
            // We do, so set the table according to display size
            _game.GameController.SetTable(new[]
            {
                    new PointF(0, 0),
                    new PointF(screenWidth, 0),
                    new PointF(0, screenHeight),
                    new PointF(screenWidth, screenHeight)
            }, GameMode);
            _recordPlayer = new RecordPlayer(this);
        }

        public void ShowEndGameScreen()
        {
            // Start depositing the data to database
            var preferences = GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);

            //Check if sync is on
            var syncSwitchDefault = Resources.GetBoolean(Resource.Boolean.saved_sync_enabled_default);
            var syncOn = preferences.GetBoolean(GetString(Resource.String.saved_sync_enabled), syncSwitchDefault);

            var team1DefaultValue = Resources.GetString(Resource.String.saved_team1_name_default);
            var team2DefaultValue = Resources.GetString(Resource.String.saved_team2_name_default);
            var team1Name = preferences.GetString(GetString(Resource.String.saved_team1_name), team1DefaultValue);
            var team2Name = preferences.GetString(GetString(Resource.String.saved_team2_name), team2DefaultValue);

            _gameEnd = true;
            // Terminate recognition
            BallColorSelected = false;

            //Hide score management buttons
            _addScoreTeam1.Visibility = ViewStates.Gone;
            _addScoreTeam2.Visibility = ViewStates.Gone;
            _removeScoreTeam1.Visibility = ViewStates.Gone;
            _removeScoreTeam2.Visibility = ViewStates.Gone;

            // Hide guideline arrows
            _arrowBot.Visibility = ViewStates.Gone;
            _arrowTop.Visibility = ViewStates.Gone;
            _arrowRight.Visibility = ViewStates.Gone;
            _arrowLeft.Visibility = ViewStates.Gone;
            // Hide top bar
            _topBar.Visibility = ViewStates.Gone;
            // Hide game button
            _gameButton.Visibility = ViewStates.Gone;
            // Hide all EmguCV drawables    
            _surfaceView.Visibility = ViewStates.Invisible;

            // Disable sensors
            if (GameMode == ECaptureMode.Live)
                _positionManager.StopListening();
            else if (!_recordPlayer.Disposed)
            {
                _recordPlayer.Stop();
                _recordPlayer.Reset();
                _recordPlayer.Release();
            }
                
            //Collect data from GameController
            MatchInfo.SetUp(_team1Title.Text, _game.GameController.BlueScore,
                            _team2Title.Text, _game.GameController.RedScore,
                            _game.GameController.MaxSpeed,
                            _game.GameController.AverageSpeed,
                            _game.GameController.HeatmapZones, _game.GameTimer.GetFormattedTime(),
                            _game.GameController.Goals);

            // Play the game end sound
            if (MatchInfo.Team1Score > MatchInfo.Team2Score)
                _game.SoundAlerts?.Play(EAlert.Team1Win);
            else
                if (MatchInfo.Team2Score > MatchInfo.Team1Score)
                _game.SoundAlerts?.Play(EAlert.Team2Win);

            // Show pop-up fragment, holding all of the match's info
            FragmentManager.BeginTransaction()
                           .Add(Resource.Id.infoLayout, EndGameFragment.NewInstance())
                           .Commit();

            // Send Data to database
            if (syncOn)
            {
                Task.Factory.StartNew(() => DatabaseManager.InsertAll(team1Name, team2Name, 
                _game.GameController.BlueScore, _game.GameController.RedScore, MatchInfo.Duration), TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Set the instances according to the layout, defined in Resources/layout/activity_game.axml
        /// </summary>
        private void GetReferencesFromLayout()
        {
            _topBar = FindViewById<RelativeLayout>(Resource.Id.fullscreen_content_top_status_bar);
            _infoLayout = FindViewById<FrameLayout>(Resource.Id.infoLayout);
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            GameView = FindViewById<TextureView>(Resource.Id.game_content);
            _score = FindViewById<TextView>(Resource.Id.score);
            _surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            _arrowTop = FindViewById<ImageView>(Resource.Id.arrowTop);
            _arrowLeft = FindViewById<ImageView>(Resource.Id.arrowLeft);
            _arrowRight = FindViewById<ImageView>(Resource.Id.arrowRight);
            _arrowBot = FindViewById<ImageView>(Resource.Id.arrowBot);
            _eventText = FindViewById<TextView>(Resource.Id.eventSlider);
            _ballSpeed = FindViewById<TextView>(Resource.Id.ballSpeed);
            _timer = FindViewById<TextView>(Resource.Id.timer);
            _team1Title = FindViewById<TextView>(Resource.Id.team1Label);
            _team2Title = FindViewById<TextView>(Resource.Id.team2Label);
            _addScoreTeam1 = FindViewById<Button>(Resource.Id.addScoreTeam1);
            _addScoreTeam2 = FindViewById<Button>(Resource.Id.addScoreTeam2);
            _removeScoreTeam1 = FindViewById<Button>(Resource.Id.removeScoreTeam1);
            _removeScoreTeam2 = FindViewById<Button>(Resource.Id.removeScoreTeam2);
        }

        public bool DetectBall(Canvas canvas) {
            return _game.ObjectDetector.Detect(canvas, _selectedBallColor,
                                            GameView.GetBitmap(PreviewWidth, PreviewHeight));
        }

        /// <summary>
        /// This function is called whenever the screen is touched
        /// </summary>
        /// <param name="v">The view, which was touched</param>
        /// <param name="e">The event class, holding the information</param>
        /// <returns>True if the Touch was accepted. False otherwise</returns>
        public bool OnTouch(View v, MotionEvent e)
        {
            // If game has ended, ignore touch
            if (_gameEnd || BallColorSelected) return false;
            // If game is not started, take sample image
            _image = new Image<Hsv, byte>(GameView.GetBitmap(PreviewWidth, PreviewHeight));
            UpdateButton(e);
            return true;
        }

        /// <summary>
        /// Draw the button using the attribute selectedHsv
        /// </summary>
        /// <param name="e">Holds the position of the Hsv value</param>
        private void UpdateButton(MotionEvent e)
        {
            // Calculate the position
            var positionX = (int)(e.GetX() / _upscaleMultiplierX);
            var positionY = (int)(e.GetY() / _upscaleMultiplierY);

            // Get the Hsv value from the image
            // Todo: exception pops HERE
            _selectedBallColor = _image[positionY, positionX];
            // convert hsv image to rgb image sample
            double hue = 0, saturation = 0, value = 0;
            int zonesFromCenter = ColorZoneCount, counter = 0;
            for (var i = -1 * zonesFromCenter; i <= zonesFromCenter; i ++)
            {
                for (var j = -1 * zonesFromCenter - 1; j <= zonesFromCenter; j ++)
                {
                    if (positionY + i <= 0 || positionY + i >= _image.Size.Width || positionX + j <= 0 ||
                        positionX + j >= _image.Size.Height) continue;
                    counter++;
                    hue += _image[positionY + i,positionX + j].Hue;
                    saturation += _image[positionY + i, positionX + j].Satuation;
                    value += _image[positionY + i, positionX + j].Value;
                }
            }
            _selectedBallColor = new Hsv(hue / counter, saturation / counter, value / counter);
            // image won't be used anymore
            _image.Dispose();
            // Convert emgu rgb to android rgb
            var selectedColor = Color.HSVToColor(new[] {
                ((float)_selectedBallColor.Hue * 2),
                ((float)_selectedBallColor.Satuation / 180),
                ((float)_selectedBallColor.Value / 180)
            });

            _gameButton.SetBackgroundColor(selectedColor);
            _gameButton.Text = GetString(Resource.String.start_game);
        }

        /// <summary>
        /// Called whenever the _gameButton is clicked
        /// </summary>
        public void GameButtonClicked(object sender, EventArgs e)
        {
            if (_gameButton.Text == GetString(Resource.String.end_game))
            {
                _game.GameTimer.Stop();
                ShowEndGameScreen();
                return;
            }

            if (_image == null) return;

            BallColorSelected = true;

            // Set the ball trace color
            _game.ObjectDetector.SetColor(_selectedBallColor);

            // Cleanup
            _image.Dispose();

            // If it's a video, start it again
            _recordPlayer?.Start();

            // Change button function to stop the game
            _gameButton.Text = GetString(Resource.String.end_game);
            _gameButton.SetBackgroundResource(Resource.Drawable.game_button_selector);

            // If game is live, capture aligned position to show guidelines accordingly
            if (GameMode == ECaptureMode.Live)
                _positionManager.CapturePosition();

            _game.GameTimer.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (GameMode == ECaptureMode.Live)
                _positionManager.StopListening();

            Finish();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _game.GameTimer.Stop();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (GameMode == ECaptureMode.Live)
                _positionManager.StartListening();
        }

        public void UpdateGuideline(bool[] exceedsPitch,
                                    bool?[] exceedsRoll = null)
        {
            _arrowBot.Visibility = exceedsPitch[0] ? ViewStates.Visible : ViewStates.Gone;
            _arrowTop.Visibility = exceedsPitch[1] ? ViewStates.Visible : ViewStates.Gone;

            // If game is not started, roll guidelines are ignored
            if (exceedsRoll == null) return;

            _arrowRight.Visibility = (exceedsRoll[0] ?? false) ? ViewStates.Visible : ViewStates.Gone;
            _arrowLeft.Visibility = (exceedsRoll[1] ?? false) ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}