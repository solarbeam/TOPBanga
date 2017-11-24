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

namespace FoosLiveAndroid
{
    //[Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    [Activity(ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity, TextureView.ISurfaceTextureListener, View.IOnTouchListener
    {
        private const int preview_width = 640;
        private const int preview_height = 360;

        private const int texture_width = 1920;
        private const int texture_height = 1080;

        private float mul_width;
        private float mul_height;

        private Button _gameButton;
        private TextView _score;
        private TextureView _gameView;

        private SurfaceView surfaceView;
        private ISurfaceHolder holder;

        private ColorDetector detector;

        private Rectangle rectangle;

        //Todo: change Camera to Camera2
        private Camera camera;
        private Hsv selectedHsv;
        private bool hsvSelected;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);
            //hides notification bar
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            GetReferencesFromLayout();

            this.mul_width = texture_width / preview_width;
            this.mul_height = texture_height / preview_height;

            this.detector = new ColorDetector();

            this.surfaceView.SetZOrderOnTop(true);
            this.surfaceView.Holder.SetFormat(Format.Transparent);
            this.holder = this.surfaceView.Holder;

            // Open the camera
            this._gameView.SurfaceTextureListener = this;
            this._gameView.SetOnTouchListener(this);
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
        {
            this.camera = Camera.Open();
            this._gameView.LayoutParameters = new FrameLayout.LayoutParams(texture_width,texture_height);

            Camera.Parameters parameters = this.camera.GetParameters();
            IList<Camera.Size> list = camera.GetParameters().SupportedPreviewSizes;

            this.holder.SetFixedSize(texture_width, texture_height);

            foreach (Camera.Size size in list)
            {
                if ( size.Width <= 1280 && size.Height <= 720 )
                {
                    parameters.SetPreviewSize(size.Width,size.Height);
                    break;
                }
            }

            this.camera.SetParameters(parameters);

            try
            {
                this.camera.SetPreviewTexture(surface);
                this.camera.StartPreview();
            }
            catch (Java.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            this.camera.StopPreview();
            this.camera.Release();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int w, int h)
        {

        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
            /**
             * Declare temporary variables
             */
            bool tableDetected = false;
            bool ballDetected = false;
            RotatedRect table;

            /**
             * Refresh the image in the detector, but
             *  of a smaller size
             */
            this.detector.image = new Image<Bgr, byte>(this._gameView.GetBitmap(preview_width,preview_height));

            /**
             * Lock the canvas for drawing
             */
            Canvas canvas = this.holder.LockCanvas();

            /**
             * Try to detect a table
             */
            if (this.detector.DetectTable(out table))
            {
                tableDetected = true;
            }

            if ( this.hsvSelected && this.detector.DetectBall(this.selectedHsv, out this.rectangle) )
            {
                ballDetected = true;
            }

            if ( tableDetected )
            {
                Console.WriteLine("Table detected");

                /**
                 * Convert PointF[] to Point[]
                 */
                System.Drawing.Point[] tablePoints = new System.Drawing.Point[4];

                for(int i = 0; i < 4; i ++)
                {
                    tablePoints[i].X = (int)table.GetVertices()[i].X;
                    tablePoints[i].Y = (int)table.GetVertices()[i].Y;
                }

                this.detector.image.DrawPolyline(tablePoints,true,new Bgr(0,0,255));
            }

            if ( ballDetected )
            {
                this.detector.image.Draw(this.rectangle, new Bgr(255, 255, 255));
            }

            canvas.DrawBitmap(this.detector.image.Bitmap,0,0,null);

            this.holder.UnlockCanvasAndPost(canvas);

            /**
             * Free unused resources
             */
            this.detector.image.Dispose();
        }

        private void GetReferencesFromLayout()
        {
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            _gameView = FindViewById<TextureView>(Resource.Id.game_content);
            _score = FindViewById<TextView>(Resource.Id.score);
            this.surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if ( !hsvSelected )
            {
                Image<Hsv, byte> image = new Image<Hsv, byte>(this._gameView.GetBitmap(preview_width, preview_height));
                this.selectedHsv = new Hsv(image.Data[ (int)(e.GetY()/mul_height), (int)(e.GetX()/mul_width), 0 ],
                                            image.Data[ (int)(e.GetY()/mul_height), (int)(e.GetX()/mul_width), 1],
                                            image.Data[ (int)(e.GetY()/mul_height), (int)(e.GetX()/mul_width), 2]);
                image.Dispose();
                this.hsvSelected = true;
            }
            return true;
        }
    }
}
