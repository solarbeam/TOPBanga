using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Emgu.CV;
using Camera = Android.Hardware.Camera;
using Emgu.CV.Structure;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Util;
using FoosLiveAndroid.Util;
using FoosLiveAndroid.Util.Detection;
using Android.Media;
using System;
using Android.Hardware;
using FoosLiveAndroid.Util.Drawing;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using FoosLiveAndroid.Util.GameControl;
using FoosLiveAndroid.Util.Sensors;
using FoosLiveAndroid.Util.Interface;
using FoosLiveAndroid.Fragments;
using FoosLiveAndroid.Util.Model;
using FoosLiveAndroid.Util.Sounds;
using FoosLiveAndroid.Model;
using Android.Support.V7.App;
using FoosLiveAndroid.Util.Record;

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
        private static readonly int SlidingTextDelay = PropertiesManager.GetIntProperty("sliding_text_delay");
        private static readonly int TimerFrequency = PropertiesManager.GetIntProperty("timer_frequency");
        private static readonly float FormatSpeed = PropertiesManager.GetFloatProperty("format_speed");

        private bool _textThreadStarted = false;
        private bool _waitForSpeed = false;
        // A constant for upscaling the positions
        private float _upscaleMultiplierX;
        private float _upscaleMultiplierY;

        private TextView _eventText;
        private TextView _timer;
        private Button _gameButton;
        private TextView _score;
        public TextureView _gameView;
        private SurfaceView _surfaceView;
        private TextView _ballSpeed;
        private TextView _team1Title;
        private TextView _team2Title;

        private string _scoreFormat;
        private string _timerFormat;

        // Guideline UI elements
        private ImageView _arrowTop;
        private ImageView _arrowLeft;
        private ImageView _arrowRight;
        private ImageView _arrowBot;

        private FrameLayout _infoLayout;
        private RelativeLayout _topBar;

        public Bitmap _alphaBitmap;

        private SoundAlerts _soundAlerts;

        private IColorDetector _colorDetector;
        private IObjectDetector _objectDetector;
        private GameController _gameController;
        private GameTimer _gameTimer;

        public ECaptureMode _gameMode;
        private bool _gameEnd;

        // Todo: change Camera to Camera2
        private Camera _camera;

        internal SurfaceManager _surfaceManager;

        private Hsv _selectedBallColor;
        internal bool _ballColorSelected;
        private Image<Hsv, byte> _image;

        private IPositionManager _positionManager;
        internal RecordPlayer _recordPlayer;

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
                _gameMode = ECaptureMode.Recording;
            }
            else
            {
                _gameMode = ECaptureMode.Live;
                // Set up sensors & vibration
                var sensorManager = (SensorManager)GetSystemService(SensorService);
                var vibration = new Vibration((Vibrator)GetSystemService(VibratorService));
                _positionManager = new PositionManager(this, sensorManager, vibration);
            }

            SetContentView(Resource.Layout.activity_game);

            //Hide notification bar
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            //Prevent from sleeping
            Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
            GetReferencesFromLayout();

            _colorDetector = new ColorDetector();
            _gameController = new GameController();
            _gameTimer = new GameTimer(TimerFrequency);
            _surfaceManager = new SurfaceManager(this, _surfaceView.Holder);
            _gameController.GoalEvent += GameControllerGoalEvent;
            _gameController.PositionEvent += GameControllerPositionEvent;


            _surfaceView.SetZOrderOnTop(true);
            _surfaceView.Holder.SetFormat(Format.Transparent);

            _gameButton.Click += GameButtonClicked;


            // Assign the sound file paths
            var preferences = GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);

            var team1DefaultValue = Resources.GetString(Resource.String.saved_team1_name_default);
            _team1Title.Text = preferences.GetString(GetString(Resource.String.saved_team1_name), team1DefaultValue);

            var team2DefaultValue = Resources.GetString(Resource.String.saved_team2_name_default);
            _team2Title.Text = preferences.GetString(GetString(Resource.String.saved_team2_name), team2DefaultValue);

            var defaultSoundEnabledValue = Resources.GetBoolean(Resource.Boolean.saved_sound_enabled_default);
            if (preferences.GetBoolean(GetString(Resource.String.saved_sound_enabled), defaultSoundEnabledValue))
            {
                var team1WinKey = Resources.GetString(Resource.String.saved_team1_win);
                var team2WinKey = Resources.GetString(Resource.String.saved_team2_win);
                var team1GoalKey = Resources.GetString(Resource.String.saved_team1_goal);
                var team2GoalKey = Resources.GetString(Resource.String.saved_team2_goal);

                var team1WinDefaultValue = Resources.GetString(Resource.String.saved_team1_win_default);
                var team2WinDefaultValue = Resources.GetString(Resource.String.saved_team2_win_default);
                var team1GoalDefaultValue = Resources.GetString(Resource.String.saved_team1_goal_default);
                var team2GoalDefaultValue = Resources.GetString(Resource.String.saved_team2_goal_default);

                _soundAlerts = new SoundAlerts
                {
                    Team1Win = new PlayerOGG(FilePathResolver.GetFile(this, preferences.GetString(team1WinKey, team1WinDefaultValue))),
                    Team1Goal = new PlayerOGG(FilePathResolver.GetFile(this, preferences.GetString(team1GoalKey, team1GoalDefaultValue))),
                    Team2Win = new PlayerOGG(FilePathResolver.GetFile(this, preferences.GetString(team2WinKey, team2WinDefaultValue))),
                    Team2Goal = new PlayerOGG(FilePathResolver.GetFile(this, preferences.GetString(team2GoalKey, team2GoalDefaultValue)))
                };
            }

            preferences.Dispose();

            _scoreFormat = GetString(Resource.String.score_format);
            _timerFormat = GetString(Resource.String.timer_format);

            // Open the camera
            _gameView.SurfaceTextureListener = _surfaceManager;
            _gameView.SetOnTouchListener(this);
            CvInvoke.UseOptimized = true;

            // Add a timer event
            _gameTimer.OnUpdated += UpdateTimer;
        }

        internal void ReleaseResources()
        {
            // Check if we use a video file for getting frames or the camera
            if (_gameMode == ECaptureMode.Recording)
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

            // Create the ObjectDetector class for the GameActivity
            _objectDetector = new ObjectDetector(_upscaleMultiplierX, _upscaleMultiplierY, _colorDetector, _gameController);
        }

        internal void SetUpCameraMode()
        {
            // Draw the align zones
            Canvas canvas = AlignZones.DrawZones(_surfaceManager.SurfaceHolder.LockCanvas(), _gameController);
            _surfaceManager.SurfaceHolder.UnlockCanvasAndPost(canvas);

            _camera = Camera.Open();

            // Get the camera parameters in order to set the appropriate frame size
            var parameters = _camera.GetParameters();
            var list = _camera.GetParameters().SupportedPreviewSizes;

            // Go through all of the sizes until we find an appropriate one
            foreach (var size in list)
            {
                if (size.Width <= CameraWidth && size.Height <= CameraHeight)
                {
                    // The size matches or is lower than that of the constants camera_width, camera_height 
                    parameters.SetPreviewSize(size.Width, size.Height);
                    break;
                }
            }

            _camera.SetParameters(parameters);
            _camera.SetDisplayOrientation(90);

            try
            {
                /**
                 * Set the surface on which frames are drawn
                 * In this case, it's the surface, which was created for _gameview
                 */
                _camera.SetPreviewTexture(_surfaceManager.SurfaceTexture);
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
            _gameController.SetTable(new PointF[]
            {
                    new PointF(0, 0),
                    new PointF(screenWidth, 0),
                    new PointF(0, screenHeight),
                    new PointF(screenWidth, screenHeight)
            }, _gameMode);
            _recordPlayer = new RecordPlayer(this);
            return;
        }

        public void ShowEndGameScreen()
        {
            _gameEnd = true;
            // Terminate recognition
            _ballColorSelected = false;

            // Hide guideline arrows
            _arrowBot.Visibility = ViewStates.Gone;
            _arrowTop.Visibility = ViewStates.Gone;
            _arrowRight.Visibility = ViewStates.Gone;
            _arrowLeft.Visibility = ViewStates.Gone;
            //_videoPlayer
            // Hide top bar
            _topBar.Visibility = ViewStates.Gone;
            // Hide game button
            _gameButton.Visibility = ViewStates.Gone;
            // Hide all EmguCV drawables
            _surfaceView.Visibility = ViewStates.Invisible;

            // Disable sensors
            if (_gameMode == ECaptureMode.Live)
                _positionManager.StopListening();

            //Collect data from GameController
            MatchInfo.SetUp(_team1Title.Text, _gameController.BlueScore,
                            _team2Title.Text, _gameController.RedScore,
                            _gameController.MaxSpeed,
                            _gameController.AverageSpeed,
                            _gameController.heatmapZones, TimeSpan.FromMilliseconds(GameTimer.Time).TotalSeconds.ToString(_timerFormat),
                            _gameController.Goals);

            // Show pop-up fragment, holding all of the match's info
            FragmentManager.BeginTransaction()
                           .Add(Resource.Id.infoLayout, EndGameFragment.NewInstance())
                           .Commit();
        }

        /// <summary>
        /// Set the instances according to the layout, defined in Resources/layout/activity_game.axml
        /// </summary>
        private void GetReferencesFromLayout()
        {
            _topBar = FindViewById<RelativeLayout>(Resource.Id.fullscreen_content_top_status_bar);
            _infoLayout = FindViewById<FrameLayout>(Resource.Id.infoLayout);
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            _gameView = FindViewById<TextureView>(Resource.Id.game_content);
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
        }

        private void UpdateTimer(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                _timer.Text = Math.Round(GameTimer.Time / Units.MiliSecondsInSecond).ToString(_timerFormat);
            });
        }

        /// <summary>
        /// Defines a sliding text effect for a given string of text
        /// </summary>
        /// <param name="text">The text, to which the effect will be applied</param>
        private void SlideText(string text)
        {
            if (_textThreadStarted)
                return;

            _textThreadStarted = true;

            RunOnUiThread(async () =>
            {
                var temp = text;
                var tempView = new StringBuilder(temp.Length);

                for (var i = 0; i < _eventText.Length(); i++)
                {
                    tempView.Append(' ');
                }
                _eventText.Text = tempView.ToString();

                for (var i = 0; i < tempView.Length * 3; i++)
                {
                    tempView.Remove(0, 1);
                    tempView.Append(i < temp.Length ? temp[i] : ' ');

                    _eventText.Text = tempView.ToString();
                    await Task.Delay(SlidingTextDelay);
                }

                _textThreadStarted = false;
            });
        }

        public bool DetectBall(Canvas canvas) {
            return _objectDetector.Detect(canvas, _selectedBallColor,
                                            _gameView.GetBitmap(PreviewWidth, PreviewHeight),
                                          _alphaBitmap);
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
                SlideText(ApplicationContext.Resources.GetString(Resource.String.blue_team_goal));
            }
            else if (e == CurrentEvent.RedGoalOccured)
            {
                _soundAlerts?.Play(EAlert.Team2Goal);
                SlideText(ApplicationContext.Resources.GetString(Resource.String.red_team_goal));
            }
            _score.Text = $"{_gameController.BlueScore} : {_gameController.RedScore}";
            Log.Debug(Tag, $"Score value assigned {_score.Text}");
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
                RunOnUiThread(async () =>
                {
                    await Task.Delay(80);
                    _waitForSpeed = false;
                });
            }
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
            if (_gameEnd) return false;
            // If game is not started, take sample image
            if (_gameButton.Visibility != ViewStates.Gone && _ballColorSelected != true)
            {
                _image = new Image<Hsv, byte>(_gameView.GetBitmap(PreviewWidth, PreviewHeight));
                UpdateButton(e);
            }
            return true;
        }

        /// <summary>
        /// Draw the button using the attribute selectedHsv
        /// </summary>
        /// <param name="e">Holds the position of the Hsv value</param>
        private void UpdateButton(MotionEvent e)
        {
            // Calculate the position
            var positionX = (int)(e.GetX() / _upscaleMultiplierX) - 1;
            var positionY = (int)(e.GetY() / _upscaleMultiplierY) - 1;

            // Get the Hsv value from the image
            // Todo: exception pops HERE
            _selectedBallColor = _image[positionY, positionX];
            // convert hsv image to rgb image sample
            var selectedRgb = _image.Convert<Rgb, byte>()[positionY, positionX];
            // image won't be used anymore
            _image.Dispose();
            // Convert emgu rgb to android rgb
            var selectedColor = Color.Rgb((int)selectedRgb.Red, (int)selectedRgb.Green, (int)selectedRgb.Blue);

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
                _gameTimer.Stop();
                ShowEndGameScreen();
                return;
            }

            if (_image == null) return;

            _ballColorSelected = true;

            // Cleanup
            _image.Dispose();

            // If it's a video, start it again
            _recordPlayer?.Start();

            // Change button function to stop the game
            _gameButton.Text = GetString(Resource.String.end_game);
            _gameButton.SetBackgroundResource(Resource.Drawable.game_button_selector);

            // If game is live, capture aligned position to show guidelines accordingly
            if (_gameMode == ECaptureMode.Live)
                _positionManager.CapturePosition();

            _gameTimer.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (_gameMode == ECaptureMode.Live)
                _positionManager.StopListening();

        }

        protected override void OnStop()
        {
            base.OnStop();
            _gameTimer.Stop();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_gameMode == ECaptureMode.Live)
                _positionManager.StartListening();
        }

        public void UpdateGuideline(bool[] exceedsPitch,
                                    bool?[] exceedsRoll = null)
        {
            _arrowBot.Visibility = (exceedsPitch[0]) ? ViewStates.Visible : ViewStates.Gone;
            _arrowTop.Visibility = (exceedsPitch[1]) ? ViewStates.Visible : ViewStates.Gone;

            // If game is not started, roll guidelines are ignored
            if (exceedsRoll == null) return;

            _arrowRight.Visibility = (exceedsRoll[0] ?? false) ? ViewStates.Visible : ViewStates.Gone;
            _arrowLeft.Visibility = (exceedsRoll[1] ?? false) ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}