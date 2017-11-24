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
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace FoosLiveAndroid
{
    //[Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    [Activity(ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity, TextureView.ISurfaceTextureListener, View.IOnTouchListener
    {
        private Button _gameButton;
        private TextView _score;
        private TextureView _gameView;
        private SurfaceView surfaceView;

        private ColorDetector detector;
        private bool calc = false;

        //Todo: change Camera to Camera2
        private Camera camera;
        private Hsv selectedHsv;
        private bool hsvSelected;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);
            //hides notification bar
            Window.SetFlags(WindowManagerFlags.HardwareAccelerated, WindowManagerFlags.HardwareAccelerated);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            GetReferencesFromLayout();

            this.detector = new ColorDetector();

            this.surfaceView.SetZOrderOnTop(true);

            // Open the camera
            this._gameView.SurfaceTextureListener = this;
            this._gameView.SetOnTouchListener(this);
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
        {
            this.camera = Camera.Open();

            System.Console.WriteLine("Width: " + w + " Height: " + h);
            this._gameView.LayoutParameters = new FrameLayout.LayoutParams(w,h);

            Camera.Parameters parameters = this.camera.GetParameters();
            IList<Camera.Size> list = camera.GetParameters().SupportedPreviewSizes;

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
            if (this.hsvSelected)
            {
                this.calc = false;
                this.detector.image = new Image<Bgr, byte>(this._gameView.GetBitmap(320,240));
                Console.WriteLine(this.detector.DetectBall(out float x, out float y, out float radius, out Bitmap bitmap, this.selectedHsv));
            }
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
                Image<Bgr, byte> image = new Image<Bgr, byte>(this._gameView.Bitmap);
                this.selectedHsv = new Hsv(image.Data[(int)e.GetY(), (int)e.GetX(), 0],
                                            image.Data[(int)e.GetY(), (int)e.GetX(), 1],
                                            image.Data[(int)e.GetY(), (int)e.GetX(), 2]);
                this.hsvSelected = true;
            }
            return true;
        }
    }
}
