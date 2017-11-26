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

        /**
         * A constant for upscaling the positions
         */
        private float mul;

        private Button _gameButton;
        private TextView _score;
        private TextureView _gameView;

        private SurfaceView surfaceView;
        private ISurfaceHolder holder;

        private Bitmap alphaBitmap;

        private ColorDetector detector;

        private Rectangle rectangle;

        //Todo: change Camera to Camera2
        private Camera camera;
        private Bgr selectedBgr;
        private bool bgrSelected;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);
            //hides notification bar
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            GetReferencesFromLayout();

            this.mul = texture_width / preview_width;

            this.detector = new ColorDetector();

            this.surfaceView.SetZOrderOnTop(true);
            this.surfaceView.Holder.SetFormat(Format.Transparent);
            this.holder = this.surfaceView.Holder;

            BitmapDrawable tempBitmap = new BitmapDrawable(Bitmap.CreateBitmap(texture_width, texture_height, Bitmap.Config.Argb8888));
            tempBitmap.SetAlpha(0);
            this.alphaBitmap = tempBitmap.Bitmap;

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
             * Refresh the detector's image
             */
            this.detector.image = new Image<Bgr, byte>(this._gameView.GetBitmap(preview_width,preview_height));

            /**
             * Lock the canvas for drawing
             */
            Canvas canvas = this.holder.LockCanvas();

            /**
             * Clear the image
             */
            canvas.DrawColor(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            canvas.DrawBitmap(this.alphaBitmap,0,0,null);

            /**
             * Try to detect a table
             */
            if (this.detector.DetectTable(out table))
            {
                tableDetected = true;
            }

            if ( this.bgrSelected && this.detector.DetectBall(this.selectedBgr, out this.rectangle) )
            {
                ballDetected = true;
            }

            /**
             *
             */
            Paint paintRect = new Paint();
            paintRect.Color = new Android.Graphics.Color(255,0,0);
            paintRect.SetStyle(Paint.Style.Stroke);

            Paint paintBall = new Paint();
            paintBall.Color = new Android.Graphics.Color(0, 255, 0);
            paintBall.SetStyle(Paint.Style.Stroke);

            if ( tableDetected )
            {
                /**
                 * Convert PointF[] to Point[]
                 */
                float[] tablePoints = new float[8];

                int j = 0;

                for(int i = 0; i < 8; i += 2)
                {
                    tablePoints[i] = table.GetVertices()[j].X * mul;
                    tablePoints[i+1] = table.GetVertices()[j].Y * mul;
                    j++;
                }

                /**
                 * Draw the rectangle
                 */
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
            if ( !this.bgrSelected )
            {
                Image<Bgr, byte> image = new Image<Bgr, byte>(this._gameView.GetBitmap(preview_width, preview_height));
                this.selectedBgr = new Bgr(image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 0 ],
                                            image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 1],
                                            image.Data[ (int)(e.GetY()/mul), (int)(e.GetX()/mul), 2]);
                image.Dispose();
                this.bgrSelected = true;
                Console.WriteLine(this.selectedBgr.ToString());
            }
            return true;
        }
    }
}
