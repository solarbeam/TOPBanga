using System;
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
using Android.Runtime;
using System.Drawing;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;
using Android.Util;
using FoosLiveAndroid.Util.Detection;
using Android.Media;

namespace FoosLiveAndroid
{
    //[Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    [Activity(ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity, TextureView.ISurfaceTextureListener, View.IOnTouchListener, MediaPlayer.IOnPreparedListener
    {
        public static string Tag = "GameActivity";
        private const int camera_width = 1280;
        private const int camera_height = 720;
        private const int preview_width = 240;
        private const int preview_height = 135;

        // A constant for upscaling the positions
        private float mul;

        private Button _gameButton;
        private TextView _score;
        private TextureView _gameView;
        private SurfaceView surfaceView;

        private ISurfaceHolder holder;

        private Bitmap alphaBitmap;

        private ColorDetector detector;
        private ObjectDetector objectDetector;

        private Rectangle rectangle;

        // Todo: change Camera to Camera2
        private Camera camera;

        private MediaPlayer video;
        private Surface surface;

        private Hsv selectedHsv;
        private bool hsvSelected;

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

            this.detector = new ColorDetector();

            this.surfaceView.SetZOrderOnTop(true);
            this.surfaceView.Holder.SetFormat(Format.Transparent);
            this.holder = this.surfaceView.Holder;

            // Open the camera
            this._gameView.SurfaceTextureListener = this;
            this._gameView.SetOnTouchListener(this);
        }

        /// <summary>
        /// Called whenever the SurfaceTexture is created
        /// </summary>
        /// <param name="surface">The surface, which is created, that calls this function</param>
        /// <param name="w">The width of the surface, defined as an integer</param>
        /// <param name="h">The height of the surface, defined as an integer</param>
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
        {
            this.holder.SetFixedSize(w, h);

            // Create a template alpha bitmap for repeated drawing
            BitmapDrawable tempBitmap = new BitmapDrawable(Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888));
            tempBitmap.SetAlpha(0);
            this.alphaBitmap = tempBitmap.Bitmap;

            // Create the ObjectDetector class for the GameActivity
            this.objectDetector = new ObjectDetector(this.mul, this.detector);

            // Set the upscaling constant
            this.mul = w / preview_width;

            this._gameView.LayoutParameters = new FrameLayout.LayoutParams(w, h);

            if ( Intent.Data != null )
            {
                this.surface = new Surface(surface);
                this.video = new MediaPlayer();
                this.video.SetDataSource(this.ApplicationContext, this.Intent.Data);
                this.video.SetSurface(this.surface);
                this.video.Prepare();
                this.video.SetOnPreparedListener(this);
                return;
            }

            this.camera = Camera.Open();

            // Get the camera parameters in order to set the appropriate frame size
            Camera.Parameters parameters = this.camera.GetParameters();
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

            this.camera.SetParameters(parameters);

            try
            {
                /**
                 * Set the surface on which frames are drawn
                 * In this case, it's the surface, which was created for _gameview
                 */
                this.camera.SetPreviewTexture(surface);
                this.camera.StartPreview();
            }
            catch (Java.IO.IOException e)
            {
                Log.Error(Tag, e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Called whenever the SurfaceTexture is destroyed
        /// </summary>
        /// <param name="surface">The surface, which call this function</param>
        /// <returns>Returns true if the input is accepted. False otherwise</returns>
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            if (this.video != null)
                this.video.Release();
            else
                this.camera.Release();

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
            if ( this.hsvSelected )
            {
                Canvas canvas = this.holder.LockCanvas();

                if ( ! this.objectDetector.Detect(canvas, this.selectedHsv,
                                            this._gameView.GetBitmap(preview_width, preview_height),
                                            this.alphaBitmap) )
                {
                    canvas.DrawBitmap(this.alphaBitmap, 0, 0, null);
                }

                this.holder.UnlockCanvasAndPost(canvas);
            }
        }

        /// <summary>
        /// Set the instances according to the layout, defined in Resources/layout/activity_game.axml
        /// </summary>
        private void GetReferencesFromLayout()
        {
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            _gameView = FindViewById<TextureView>(Resource.Id.game_content);
            _score = FindViewById<TextView>(Resource.Id.score);
            surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
        }

        /// <summary>
        /// This function is called whenever the screen is touched
        /// </summary>
        /// <param name="v">The view, which was touched</param>
        /// <param name="e">The event class, holding the information</param>
        /// <returns>True if the Touch was accepted. False otherwise</returns>
        public bool OnTouch(View v, MotionEvent e)
        {
            if ( !this.hsvSelected )
            {
                Image<Hsv, byte> image;

                image = new Image<Hsv, byte>(this._gameView.GetBitmap(preview_width, preview_height));

                this.selectedHsv = new Hsv(image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 0 ],
                                            image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 1],
                                            image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 2]);

                // Dispose of the temporary image
                image.Dispose();
                this.hsvSelected = true;

                // If the video was paused, resume it
                if ( this.video != null )
                {
                    this.video.Start();
                }
            }
            return true;
        }

        /// <summary>
        /// Called whenever the mediaplayer is ready to be started
        /// </summary>
        /// <param name="mp">The MediaPlayer instance, which called this function</param>
        public void OnPrepared(MediaPlayer mp)
        {
            mp.Start();
            mp.Pause();
        }
    }
}
