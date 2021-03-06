﻿using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Util.Model;

namespace FoosLiveAndroid.Util.Record
{
    public class SurfaceManager : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        static readonly string Tag = typeof(SurfaceManager).Name;
        private GameActivity _activity;
        public ISurfaceHolder SurfaceHolder { get; }
        public Surface Surface { get; private set; }
        public SurfaceTexture SurfaceTexture { get; private set; }

        public SurfaceManager(Context context, ISurfaceHolder holder)
        {
            _activity = (GameActivity) context;
            SurfaceHolder = holder;
        }

        /// <summary>
        /// Called whenever the SurfaceTexture is created
        /// </summary>
        /// <param name="surface">The surface, which is created, that calls this function</param>
        /// <param name="w">The width of the surface, defined as an integer</param>
        /// <param name="h">The height of the surface, defined as an integer</param>
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
        {
            _activity.GameView.LayoutParameters = new FrameLayout.LayoutParams(w, h);

            // Set the upscaling constant
            _activity.SetMultipliers(w, h);

            SurfaceHolder.SetFixedSize(w, h);

            SurfaceTexture = surface;

            // Check if we use video mode
            if (_activity.GameMode == ECaptureMode.Recording)
            {
                Surface = new Surface(surface);
                _activity.SetUpRecordMode(w, h);
            }
            else
                _activity.SetUpCameraMode();
        }

        /// <summary>
        /// Called whenever the SurfaceTexture is destroyed
        /// </summary>
        /// <param name="surface">The surface, which call this function</param>
        /// <returns>Returns true if the input is accepted. False otherwise</returns>
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            _activity.ReleaseResources();
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
            Log.Wtf(Tag, "Surface texture size changed. It shouldn't.");
        }

        /// <summary>
        /// Called whenever the SurfaceTexture is updated
        /// </summary>
        /// <param name="surface">The surface, which calls this function</param>
        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
            // The table is currently drawn only if an Hsv value is selected
            if (!_activity.BallColorSelected) return;

            var canvas = SurfaceHolder.LockCanvas();

            if (!_activity.DetectBall(canvas))
            {
                // Remove all drawings
                canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
            }

            SurfaceHolder.UnlockCanvasAndPost(canvas);
            canvas.Dispose();
        }
    }
}
