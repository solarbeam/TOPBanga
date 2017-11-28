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
using FoosLiveAndroid.TOPBanga.Detection;
using System.Collections.Generic;
using Android.Runtime;
using System.Drawing;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;
using Android.Util;

namespace FoosLiveAndroid
{
    //[Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    [Activity(ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity, TextureView.ISurfaceTextureListener, View.IOnTouchListener
    {
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

        private Rectangle rectangle;

        // Todo: change Camera to Camera2
        private Camera camera;

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
            this.camera = Camera.Open();
            this._gameView.LayoutParameters = new FrameLayout.LayoutParams(w,h);

            this.mul = w / preview_width;

            // Create a template alpha bitmap for repeated drawing
            BitmapDrawable tempBitmap = new BitmapDrawable(Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888));
            tempBitmap.SetAlpha(0);
            this.alphaBitmap = tempBitmap.Bitmap;

            // Get the camera parameters in order to set the appropriate frame size
            Camera.Parameters parameters = this.camera.GetParameters();
            IList<Camera.Size> list = camera.GetParameters().SupportedPreviewSizes;

            this.holder.SetFixedSize(w, h);

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
            catch (Java.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Called whenever the SurfaceTexture is destroyed
        /// </summary>
        /// <param name="surface">The surface, which call this function</param>
        /// <returns>Returns true if the input is accepted. False otherwise</returns>
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            this.camera.StopPreview();
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
            // Declare temporary variables
            bool tableDetected = false;
            bool ballDetected = false;
            RotatedRect table;

            // Refresh the detector's image
            this.detector.image = new Image<Bgr, byte>(this._gameView.GetBitmap(preview_width,preview_height));

            // Lock the canvas for drawing
            Canvas canvas = this.holder.LockCanvas();

            // Clear the image
            canvas.DrawColor(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            canvas.DrawBitmap(this.alphaBitmap,0,0,null);

            // Try to detect a table
            if (this.detector.DetectTable(out table))
            {
                tableDetected = true;
            }

            if ( this.hsvSelected && this.detector.DetectBall(this.selectedHsv, out this.rectangle) )
            {
                ballDetected = true;
            }

            // Declare the outline styles
            Paint paintRect = new Paint();
            paintRect.Color = new Android.Graphics.Color(255,0,0);
            paintRect.SetStyle(Paint.Style.Stroke);

            Paint paintBall = new Paint();
            paintBall.Color = new Android.Graphics.Color(0, 255, 0);
            paintBall.SetStyle(Paint.Style.Stroke);

            if ( tableDetected )
            {
                // Get the table points
                float[] tablePoints = new float[8];

                int j = 0;

                for(int i = 0; i < 8; i += 2)
                {
                    tablePoints[i] = table.GetVertices()[j].X * mul;
                    tablePoints[i+1] = table.GetVertices()[j].Y * mul;
                    j++;
                }

                // Finally, draw the rectangle
                canvas.DrawLine(tablePoints[0], tablePoints[1], tablePoints[2], tablePoints[3], paintRect);
                canvas.DrawLine(tablePoints[2], tablePoints[3], tablePoints[4], tablePoints[5], paintRect);
                canvas.DrawLine(tablePoints[4], tablePoints[5], tablePoints[6], tablePoints[7], paintRect);
                canvas.DrawLine(tablePoints[6], tablePoints[7], tablePoints[0], tablePoints[1], paintRect);
            }

            if ( ballDetected )
            {
                canvas.DrawRect((int)((this.rectangle.X) * mul),
                                 (int)((this.rectangle.Y) * mul),
                                 (int)((this.rectangle.X + this.rectangle.Width) * mul),
                                 (int)((this.rectangle.Y + this.rectangle.Height) * mul),
                                 paintBall);
            }

            // Draw the canvas
            this.holder.UnlockCanvasAndPost(canvas);

            // Free unused resources
            this.detector.image.Dispose();
        }

        /// <summary>
        /// Set the instances according to the layout, defined in Resources/layout/activity_game.axml
        /// </summary>
        private void GetReferencesFromLayout()
        {
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            _gameView = FindViewById<TextureView>(Resource.Id.game_content);
            _score = FindViewById<TextView>(Resource.Id.score);
            this.surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
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
                Image<Hsv, byte> image = new Image<Hsv, byte>(this._gameView.GetBitmap(preview_width, preview_height));
                this.selectedHsv = new Hsv(image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 0 ],
                                            image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 1],
                                            image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 2]);

                // Dispose of the temporary image
                image.Dispose();
                this.hsvSelected = true;
                Console.WriteLine(this.selectedHsv.ToString());
            }
            return true;
        }
    }
}
