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
using Android.Runtime;
using FoosLiveAndroid.Util.Sensors;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class GameActivity : Activity, TextureView.ISurfaceTextureListener, View.IOnTouchListener, MediaPlayer.IOnPreparedListener
    {
        static readonly string Tag = typeof(GameActivity).Name;
        private readonly int camera_width = PropertiesManager.GetIntProperty("camera_width");
        private readonly int camera_height = PropertiesManager.GetIntProperty("camera_height");
        private readonly int preview_width = PropertiesManager.GetIntProperty("preview_width");
        private readonly int preview_height = PropertiesManager.GetIntProperty("preview_height");

        // A constant for upscaling the positions
        private float upscaleMultiplierX;
        private float upscaleMultiplierY;

        private Button _gameButton;
        private TextView _score;
        private TextureView _gameView;
        private SurfaceView _surfaceView;
        // Guideline UI elements
        private ImageView _arrowTop;
        private ImageView _arrowLeft;
        private ImageView _arrowRight;
        private ImageView _arrowBot;

        private ISurfaceHolder holder;

        private Bitmap alphaBitmap;

        private ColorDetector detector;
        private ObjectDetector objectDetector;
        private GameController gameController;

        private PointF rectangle;

        // Todo: change Camera to Camera2
        private Camera camera;

        private MediaPlayer video;
        private Surface surface;

        private Hsv selectedHsv;
        private bool hsvSelected;
        private Image<Hsv, byte> image;

        private IPositionManager positionManager;

        /// <summary>
        /// Called whenever the view is created
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_game);

            //hides notification bar
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            GetReferencesFromLayout();

            detector = new ColorDetector();
            gameController = new GameController();
            gameController.GoalEvent += GameController_GoalEvent;

            _surfaceView.SetZOrderOnTop(true);
            _surfaceView.Holder.SetFormat(Format.Transparent);
            holder = _surfaceView.Holder;

            _gameButton.Text = GetString(Resource.String.select_ball_color);
            _gameButton.Click += StartGame;

            // Open the camera
            _gameView.SurfaceTextureListener = this;
            _gameView.SetOnTouchListener(this);
            CvInvoke.UseOptimized = true;

            // Set up sensors & vibration
            var sensorManager = (SensorManager)GetSystemService(SensorService);
            var _vibration = new Vibration((Vibrator)GetSystemService(VibratorService));
            positionManager = new PositionManager(this, sensorManager, _vibration);
        }

        /// <summary>
        /// Set the instances according to the layout, defined in Resources/layout/activity_game.axml
        /// </summary>
        private void GetReferencesFromLayout()
        {
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            _gameView = FindViewById<TextureView>(Resource.Id.game_content);
            _score = FindViewById<TextView>(Resource.Id.score);
            _surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);

            _arrowTop = FindViewById<ImageView>(Resource.Id.arrowTop);
            _arrowLeft = FindViewById<ImageView>(Resource.Id.arrowLeft);
            _arrowRight = FindViewById<ImageView>(Resource.Id.arrowRight);
            _arrowBot = FindViewById<ImageView>(Resource.Id.arrowBot);
        }

        /// <summary>
        /// Called whenever a goal event occurs
        /// </summary>
        /// <param name="sender">The class, which called this function</param>
        /// <param name="e">Arguments, which are passed to this function</param>
        private void GameController_GoalEvent(object sender, EventArgs e)
        {
            _score.Text = gameController.BlueScore + " : " + gameController.RedScore;
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
            upscaleMultiplierY = (float)h / preview_height;
            upscaleMultiplierX = (float)w / preview_width;

            // Create the ObjectDetector class for the GameActivity
            objectDetector = new ObjectDetector(upscaleMultiplierX, upscaleMultiplierY, detector, gameController);

            // Create a template alpha bitmap for repeated drawing
            var tempBitmap = new BitmapDrawable(Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888));
            tempBitmap.SetAlpha(0);
            alphaBitmap = tempBitmap.Bitmap;

            holder.SetFixedSize(w, h);

            // Set temporary points for now
            gameController.SetTable(new PointF[]
            {
                new PointF(0,0),
                new PointF(w,0),
                new PointF(0,h),
                new PointF(w,h)
            });

            if ( Intent.Data != null )
            {
                this.surface = new Surface(surface);
                video = new MediaPlayer();
                video.SetDataSource(ApplicationContext, Intent.Data);
                video.SetSurface(this.surface);
                video.Prepare();
                video.SetOnPreparedListener(this);
                return;
            }

            camera = Camera.Open();

            // Get the camera parameters in order to set the appropriate frame size
            Camera.Parameters parameters = camera.GetParameters();
            IList<Camera.Size> list = camera.GetParameters().SupportedPreviewSizes;

            // Go through all of the sizes until we find an appropriate one
            foreach (Camera.Size size in list)
            {
                if ( size.Width <= camera_width && size.Height <= camera_height )
                {
                    // The size matches or is lower than that of the constants camera_width, camera_height 
                    parameters.SetPreviewSize(size.Width,size.Height);
                    break;
                }
            }

            camera.SetParameters(parameters);
            camera.SetDisplayOrientation(90);

            try
            {
                /**
                 * Set the surface on which frames are drawn
                 * In this case, it's the surface, which was created for _gameview
                 */
                camera.SetPreviewTexture(surface);
                camera.StartPreview();
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
            if (video != null)
                // We use a video file, so release it's resources
                video.Release();
            else
                // We use a camera, so release it
                camera.Release();

            positionManager.StopListening();
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
            if ( hsvSelected )
            {
                Canvas canvas = holder.LockCanvas();

                if ( ! objectDetector.Detect(canvas, selectedHsv,
                                            _gameView.GetBitmap(preview_width, preview_height),
                                            alphaBitmap) )
                {
                    canvas.DrawBitmap(alphaBitmap, 0, 0, null);
                }

                holder.UnlockCanvasAndPost(canvas);
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
            // if game is not started
            if ( _gameButton.Visibility != ViewStates.Gone )
            {
                //image = image ?? new Image<Hsv, byte>(_gameView.GetBitmap(preview_width, preview_height));
                image = new Image<Hsv, byte>(_gameView.GetBitmap(preview_width, preview_height));
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
            int positionX = (int)(e.GetX() / upscaleMultiplierX);
            int positionY = (int)(e.GetY() / upscaleMultiplierY);

            // Get the Hsv value from the image
            selectedHsv = image[positionY, positionX];
            // convert hsv image to rgb image sample
            var selectedRgb = image.Convert<Rgb, Byte>()[positionY, positionX];

            //Todo: check whether it causes glitches
            image.Dispose();
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

            // Pause the video to let the user choose an Hsv value
            mp.Pause();
        }

        /// <summary>
        /// Called whenever the _gameButton is clicked
        /// </summary>
        public void StartGame(object sender, EventArgs e)
        {
            if (image == null) return;

            hsvSelected = true;
            // Cleanup
            image.Dispose();

            // If it's a video, start it again
            video?.Start();

            // We don't need the button anymore, so remove it
            _gameButton.Visibility = ViewStates.Gone;

            // Base point to check roll value changes 
            positionManager.CapturePosition();
        }

        protected override void OnPause()
        {
            base.OnPause();
            positionManager.StopListening();
        }

        protected override void OnResume()
        {
            base.OnResume();
            positionManager.StartListening();
        }

        public void UpdateGuideline(bool exceedsTop, 
                                    bool exceedsBot, 
                                    bool? exceedsLeft = null, 
                                    bool? exceedsRight = null)
        {
            _arrowTop.Visibility = (exceedsBot) ? ViewStates.Visible : ViewStates.Gone;
            _arrowBot.Visibility = (exceedsTop) ? ViewStates.Visible : ViewStates.Gone;

            if (exceedsLeft == null || exceedsRight == null) return;
            _arrowLeft.Visibility = (exceedsRight ?? false) ? ViewStates.Visible : ViewStates.Gone;
            _arrowRight.Visibility = (exceedsLeft ?? false) ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}
