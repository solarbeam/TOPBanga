
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Emgu.CV;
using Camera = Android.Hardware.Camera;
using Java.Lang;
using Android.Media;
using Emgu.CV.Structure;

namespace FoosLiveAndroid
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity, TextureView.ISurfaceTextureListener, View.IOnTouchListener
    {
        private Button _gameButton;
        private TextView _score;
        private TextureView _gameView;
        private SurfaceView surfaceView;

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

            /**
             * Open the camera
             */
            this._gameView.SurfaceTextureListener = this;
            this._gameView.SetOnTouchListener(this);
        }

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
        {
            this.camera = Camera.Open();

            System.Console.WriteLine("Width: " + w + " Height: " + h);
            this._gameView.LayoutParameters = new FrameLayout.LayoutParams(w, h);

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
            
        }

        private void GetReferencesFromLayout()
        {
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            _gameView = FindViewById<TextureView>(Resource.Id.game_content);
            _score = FindViewById<TextView>(Resource.Id.score);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if ( !hsvSelected )
            {
                Image<Bgr, byte> image = new Image<Bgr, byte>(this._gameView.Bitmap);
                this.selectedHsv = new Hsv(image.Data[(int)e.GetY(), (int)e.GetX(), 0],
                                            image.Data[(int)e.GetY(), (int)e.GetX(), 1],
                                            image.Data[(int)e.GetY(), (int)e.GetX(), 2]);
                System.Console.WriteLine(this.selectedHsv.ToString());
                this.hsvSelected = true;
            }
            return true;
        }
    }
}
