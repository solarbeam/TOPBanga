using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoosLiveAndroid.Interface
{
    interface IFrameGrabber
    {
        Image<Bgr, Byte> GrabFrame();
        void Start();
        void Stop();
    }
}