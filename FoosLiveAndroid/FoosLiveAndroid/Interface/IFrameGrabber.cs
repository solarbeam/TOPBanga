using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace FoosLiveAndroid.Interface
{
    interface IFrameGrabber
    {
        Image<Bgr, Byte> GrabFrame();
        void Start();
        void Stop();
    }
}