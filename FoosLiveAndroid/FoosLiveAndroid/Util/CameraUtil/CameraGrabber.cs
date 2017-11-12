using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Interface;
using Android.Hardware.Camera2;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using Emgu.CV;
using Emgu.CV.Structure;
using Java.Lang;
using Android.Media;
using Android.Util;
using Java.Util.Concurrent;

namespace FoosLiveAndroid.Util.CameraUtil
{
    partial class CameraGrabber : IFrameGrabber
    {
        private const int width = 1920;
        private const int height = 1080;
        private int maxImages = 2;
        private AutoFitTextureView textureView;
        private Semaphore openCloseCamera;
        private CameraManager manager;
        CameraDevice camera;
        string cameraID;
        public CameraGrabber(int width, int height, Activity activity, View view)
        {
            /**
             * Cast view into an AutoFitTextureView
             */
            //this.textureView = (AutoFitTextureView)view.FindViewById(Resource.Id.texture);

            CameraOutputs.SetUp(activity,ImageReader.NewInstance(width,height,Android.Graphics.ImageFormatType.Jpeg,maxImages),
                width,height, out Size previewSize,this.textureView,out string cameraID, out bool flashSupported);
            CameraTransform.ConfigureTransform(width, height, activity, textureView, previewSize);

            this.manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            this.openCloseCamera = new Semaphore(1);
            try
            {
                if (!this.openCloseCamera.TryAcquire(2500, TimeUnit.Microseconds))
                {
                    throw new RuntimeException("Time out waiting to lock camera opening.");
                }
                /**
                 * Assign callback and handler
                 */
                //manager.OpenCamera(cameraID, mStateCallback, mBackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
            catch (InterruptedException e)
            {
                throw new RuntimeException("Interrupted while trying to lock camera opening.", e);
            }
        }
        public Image<Bgr, byte> GrabFrame()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}