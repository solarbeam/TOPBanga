using Android.OS;

namespace FoosLiveAndroid.Util
{
    public class Vibration
    {
        private readonly Vibrator _vibrator;
        private bool _vibrating = false;
        private readonly int vibrationRepeatIndex = PropertiesManager.GetIntProperty("vibration_repeat_index");
        private readonly long[] _vibrationPattern =
{
            PropertiesManager.GetIntProperty("vibration_pattern_timing1"),
            PropertiesManager.GetIntProperty("vibration_pattern_timing2"),
            PropertiesManager.GetIntProperty("vibration_pattern_timing3")
        };

        public Vibration(Vibrator vibrator)
        {
            _vibrator = vibrator;
        }

        public void Start()
        {
            if (_vibrating) return;
            if ((int)Build.VERSION.SdkInt >= 26)
            {
                _vibrator.Vibrate(VibrationEffect.CreateWaveform(_vibrationPattern, vibrationRepeatIndex));
            }
            else 
            {
                #pragma warning disable CS0618 // Type or member is obsolete
                _vibrator.Vibrate(_vibrationPattern, 0);
                #pragma warning restore CS0618 // Type or member is obsolete
            }
            _vibrating = true;
        }

        public void Stop()
        {
            if (!_vibrating) return;
            _vibrator.Cancel();
            _vibrating = false;
        }
    }
}
