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

namespace FoosLiveAndroid.Util.CameraUtil
{
    partial class CameraGrabber : IFrameGrabber
    {
        CameraDevice camera;
        public CameraGrabber(int width, int height, Activity activity)
        {
            CameraManager manager = (CameraManager)activity.GetSystemService(Context.CameraService);
            try
            {

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