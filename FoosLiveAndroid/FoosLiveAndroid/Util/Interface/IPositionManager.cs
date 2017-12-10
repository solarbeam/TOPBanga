using System;
namespace FoosLiveAndroid.Util.Interface
{
    public interface IPositionManager
    {
        void CapturePosition();
        void StartListening();
        void StopListening();
    }
}
