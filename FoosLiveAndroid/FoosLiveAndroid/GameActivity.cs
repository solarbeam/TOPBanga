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

namespace FoosLiveAndroid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : AppCompatActivity, TextureView.ISurfaceTextureListener, View.IOnTouchListener, MediaPlayer.IOnPreparedListener,
            MediaPlayer.IOnCompletionListener
    {
        static readonly string Tag = typeof(GameActivity).Name;
        private static readonly int CameraWidth = PropertiesManager.GetIntProperty("camera_width");
        private static readonly int CameraHeight = PropertiesManager.GetIntProperty("camera_height");
        private static readonly int PreviewWidth = PropertiesManager.GetIntProperty("preview_width");
        private static readonly int PreviewHeight = PropertiesManager.GetIntProperty("preview_height");
        private static readonly int SlidingTextDelay = PropertiesManager.GetIntProperty("sliding_text_delay");
        private static readonly int TimerFrequency = PropertiesManager.GetIntProperty("timer_frequency");
        private static readonly float FormatSpeed = PropertiesManager.GetFloatProperty("format_speed");
        private const double miliSecondsInSecond = 1000;
        private bool _textThreadStarted = false;
        private bool _waitForSpeed = false;

        // A constant for upscaling the positions
        private float _upscaleMultiplierX;
        private float _upscaleMultiplierY;

        private TextView _eventText;
        private TextView _timer;
        private Button _gameButton;
        private TextView _score;
        private TextureView _gameView;
        private SurfaceView _surfaceView;
        private TextView _ballSpeed;

        private string scoreFormat;
        private string timerFormat;

        // Guideline UI elements
        private ImageView _arrowTop;
        private ImageView _arrowLeft;
        private ImageView _arrowRight;
        private ImageView _arrowBot;

        private FrameLayout _infoLayout;
        private RelativeLayout _topBar;

        private ISurfaceHolder _surfaceHolder;

        private Bitmap _alphaBitmap;

        private SoundAlerts _soundAlerts;

        private IColorDetector _colorDetector;
        private IObjectDetector _objectDetector;
        private GameController _gameController;
        private GameTimer _gameTimer;

        private ECaptureMode _gameMode;
        private bool _gameEnd;

        // Todo: change Camera to Camera2
        private Camera _camera;

        private bool _videoDisposed = true; 
        private MediaPlayer _video;
        private Surface _surface;

        private Hsv _selectedHsv;
        private bool _hsvSelected;
        private Image<Hsv, byte> _image;

        private IPositionManager _positionManager;

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
            _gameController.GoalEvent += GameControllerGoalEvent;
            _gameController.PositionEvent += GameControllerPositionEvent;
            _gameTimer = new GameTimer(TimerFrequency);

            _surfaceView.SetZOrderOnTop(true);
            _surfaceView.Holder.SetFormat(Format.Transparent);
            _surfaceHolder = _surfaceView.Holder;

            _gameButton.Text = GetString(Resource.String.select_ball_color);
            _gameButton.Click += GameButtonClicked;

            // Assign the sound file paths
            ISharedPreferences prefs = GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);

            if (prefs.GetBoolean("soundEnabled", true))
            {
                _soundAlerts = new SoundAlerts
                {
                    BlueTeamWins = new PlayerOGG(this, FilePathResolver.getFile(this, prefs.GetString("team1Win", ""))),
                    BlueTeamGoal = new PlayerOGG(this, FilePathResolver.getFile(this, prefs.GetString("team1Score", ""))),
                    RedTeamWins = new PlayerOGG(this, FilePathResolver.getFile(this, prefs.GetString("team2Win", ""))),
                    RedTeamGoal = new PlayerOGG(this, FilePathResolver.getFile(this, prefs.GetString("team2Score", "")))
                };
            }

            prefs.Dispose();

            scoreFormat = GetString(Resource.String.score_format);
            timerFormat = GetString(Resource.String.timer_format);

            // Open the camera
            _gameView.SurfaceTextureListener = this;
            _gameView.SetOnTouchListener(this);
            CvInvoke.UseOptimized = true;

            // Add a timer event
            _gameTimer.OnUpdated += UpdateTimer;
        }

        private void ShowEndGameScreen()
        {
            _gameEnd = true;
            // Terminate recognition
            _hsvSelected = false;

            // Hide guideline arrows
            _arrowBot.Visibility = ViewStates.Gone;
            _arrowTop.Visibility = ViewStates.Gone;
            _arrowRight.Visibility = ViewStates.Gone;
            _arrowLeft.Visibility = ViewStates.Gone;

            // Remove drawable lefovers
            Canvas canvas = _surfaceHolder.LockCanvas();
            canvas.DrawBitmap(_alphaBitmap, 0, 0, null);
            _surfaceHolder.UnlockCanvasAndPost(canvas);

            // Hide top bar
            _topBar.Visibility = ViewStates.Gone;
            // Hide game button
            _gameButton.Visibility = ViewStates.Gone;

            // Disable sensors
            if (_gameMode == ECaptureMode.Live)
                _positionManager.StopListening();
            
            //Collect data from GameController
            MatchInfo.SetUp("Team 1", _gameController.BlueScore,
                            "Team 2", _gameController.RedScore,
                            _gameController.MaxSpeed,
                            _gameController.AverageSpeed,
                            _gameController.heatmapZones, TimeSpan.FromMilliseconds(GameTimer.Time).TotalSeconds.ToString(timerFormat),
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
        }

        private void UpdateTimer(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                _timer.Text = Math.Round(GameTimer.Time / miliSecondsInSecond).ToString(timerFormat);
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

                for (var i = 0; i < tempView.Length * 3; i ++)
                {
                    tempView.Remove(0, 1);
                    tempView.Append(i < temp.Length ? temp[i] : ' ');

                    _eventText.Text = tempView.ToString();
                    await Task.Delay(SlidingTextDelay);
                }

                _textThreadStarted = false;
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
                _soundAlerts?.Play(EAlert.BlueGoal);
                SlideText(ApplicationContext.Resources.GetString(Resource.String.blue_team_goal));
            }
            else
                if (e == CurrentEvent.RedGoalOccured)
            {
                _soundAlerts?.Play(EAlert.RedGoal);
                SlideText(ApplicationContext.Resources.GetString(Resource.String.red_team_goal));
            }

            _score.Text = String.Format(scoreFormat, _gameController.BlueScore, _gameController.RedScore);
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
        /// Called whenever the SurfaceTexture is created
        /// </summary>
        /// <param name="surface">The surface, which is created, that calls this function</param>
        /// <param name="w">The width of the surface, defined as an integer</param>
        /// <param name="h">The height of the surface, defined as an integer</param>
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
        {
            _gameView.LayoutParameters = new FrameLayout.LayoutParams(w, h);

            // Set the upscaling constant
            _upscaleMultiplierY = (float)h / PreviewHeight;
            _upscaleMultiplierX = (float)w / PreviewWidth;

            // Create the ObjectDetector class for the GameActivity
            _objectDetector = new ObjectDetector(_upscaleMultiplierX, _upscaleMultiplierY, _colorDetector, _gameController);

            // Create a template alpha bitmap for repeated drawing
            var tempBitmap = new BitmapDrawable(Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888));
            tempBitmap.SetAlpha(0);
            _alphaBitmap = tempBitmap.Bitmap;

            _surfaceHolder.SetFixedSize(w, h);

            // Check if we use video mode
            if (_gameMode == ECaptureMode.Recording)
            {
                // We do, so set the table according to display size
                _gameController.SetTable(new PointF[]
                {
                    new PointF(0,0),
                    new PointF(w,0),
                    new PointF(0,h),
                    new PointF(w,h)
                }, _gameMode);

                _surface = new Surface(surface);
                _video = new MediaPlayer();
                _videoDisposed = false;
                _video.SetDataSource(ApplicationContext, Intent.Data);
                _video.SetSurface(_surface);
                _video.Prepare();
                _video.SetOnPreparedListener(this);
                _video.SetOnCompletionListener(this);
                return;
            }

            // Draw the align zones
            Canvas canvas = AlignZones.DrawZones(_surfaceHolder.LockCanvas(), _gameController);
            _surfaceHolder.UnlockCanvasAndPost(canvas);
            _camera = Camera.Open();

            // Get the camera parameters in order to set the appropriate frame size
            Camera.Parameters parameters = _camera.GetParameters();
            IList<Camera.Size> list = _camera.GetParameters().SupportedPreviewSizes;

            // Go through all of the sizes until we find an appropriate one
            foreach (Camera.Size size in list)
            {
                if ( size.Width <= CameraWidth && size.Height <= CameraHeight )
                {
                    // The size matches or is lower than that of the constants camera_width, camera_height 
                    parameters.SetPreviewSize(size.Width,size.Height);
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
                _camera.SetPreviewTexture(surface);
                _camera.StartPreview();
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(Tag, e.Message);
                throw;
            }
        }

        /// <summary>
        /// Called whenever the SurfaceTexture is destroyed
        /// </summary>
        /// <param name="surface">The surface, which call this function</param>
        /// <returns>Returns true if the input is accepted. False otherwise</returns>
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            // Check if we use a video file for getting frames or the camera
            if (_video != null && !_videoDisposed)
            {
                // We use a video file, so release it's resources
                _video.Release();
                _video.Dispose();
                _videoDisposed = true;
            }
            else
                // We use a camera, so release it
                _camera?.Release();

            _positionManager?.StopListening();
            return true;
        }

        /// <summary>
        /// Called whenever the SurfaceTexture's size is changed
        /// </summary>
        /// <param name="surface">The surface, which called this function</param>
        /// <param name="w">The new width, defined as an integer</param>
        /// <param name="h">The new height, defined as an integer</param>
        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int w, int h)
        {

        }

        /// <summary>
        /// Called whenever the SurfaceTexture is updated
        /// </summary>
        /// <param name="surface">The surface, which calls this function</param>
        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
            // The table is currently drawn only if an Hsv value is selected
            if ( _hsvSelected )
            {
                Canvas canvas = _surfaceHolder.LockCanvas();

                if ( ! _objectDetector.Detect(canvas, _selectedHsv,
                                            _gameView.GetBitmap(PreviewWidth, PreviewHeight),
                                            _alphaBitmap) )
                {
                    // Remove all drawings
                    canvas.DrawBitmap(_alphaBitmap, 0, 0, null);
                }

                _surfaceHolder.UnlockCanvasAndPost(canvas);
                canvas.Dispose();
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
            if ( _gameButton.Visibility != ViewStates.Gone && _hsvSelected != true )
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
            var positionX = (int)(e.GetX() / _upscaleMultiplierX);
            var positionY = (int)(e.GetY() / _upscaleMultiplierY);

            // Get the Hsv value from the image
            _selectedHsv = _image[positionY, positionX];
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
        /// Called whenever the mediaplayer is ready to be started
        /// </summary>
        /// <param name="mp">The MediaPlayer instance, which called this function</param>
        public void OnPrepared(MediaPlayer mp)
        {
            mp.Start();

            // We only need the frames from the video, so mute the sound
            mp.SetVolume(0, 0);
            mp.PlaybackParams = mp.PlaybackParams.SetSpeed(1.0f);

            // Pause the video to let the user choose an Hsv value
            mp.Pause();
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

            _hsvSelected = true;

            // Cleanup
            _image.Dispose();

            // If it's a video, start it again
            _video?.Start();

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

        public void OnCompletion(MediaPlayer mp)
        {
            if (!_videoDisposed)
            {
                mp.Release();
                mp.Dispose();
                _videoDisposed = true;
            }
            ShowEndGameScreen();
        }
    }
}