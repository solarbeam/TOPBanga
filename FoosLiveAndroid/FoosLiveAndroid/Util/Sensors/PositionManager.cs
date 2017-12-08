using System;
using Android.Hardware;
using Android.Runtime;

namespace FoosLiveAndroid.Util.Sensors
{
    public class PositionManager : Java.Lang.Object, ISensorEventListener
    {

        private Vibration _vibration;
        private SensorManager _sensorManager;
        private Sensor _rotationSensor;
        private SensorStatus _lastAccuracy;

        private readonly int PitchOffset = PropertiesManager.GetIntProperty("pitch_offset");
        private readonly int RollOffset = PropertiesManager.GetIntProperty("roll_offset");
        private readonly int SuggestedPitchMin = PropertiesManager.GetIntProperty("suggested_pitch_min");
        private readonly int SuggestedPitchMax = PropertiesManager.GetIntProperty("suggested_pitch_max");
        private readonly int MaxRollDeviaton = PropertiesManager.GetIntProperty("max_roll_deviation");

        private float _pitch;
        private float _roll;
        private float _referencePointRoll;

        private GameActivity _activity;

        private bool gameStarted = false;

        public PositionManager(GameActivity activity, SensorManager sensorManager, Vibration vibration)
        {
            _activity = activity;
            _rotationSensor = sensorManager.GetDefaultSensor(SensorType.RotationVector);
            _vibration = vibration;
        }

        /// <summary>
        /// Called on change of sensors accuracy
        /// </summary>
        /// <param name="sensor">Sensor</param>
        /// <param name="accuracy">Accuracy</param>
        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            _lastAccuracy = accuracy;
        }

        /// <summary>
        /// Called on change of sensors values
        /// </summary>
        /// <param name="e">Sensor event</param>
        public void OnSensorChanged(SensorEvent e)
        {
            // Do not handle low accuracy / unreliable data
            if (_lastAccuracy == SensorStatus.AccuracyLow || _lastAccuracy == SensorStatus.Unreliable) return;

            if (e.Sensor.Type == SensorType.RotationVector)
            {
                float[] rotationMatrix = new float[9];
                float[] rotationVector = new float[e.Values.Count];

                // extract raw data
                for (int i = 0; i < rotationVector.Length; i++)
                    rotationVector[i] = e.Values[i];

                // parse raw data
                SensorManager.GetRotationMatrixFromVector(rotationMatrix, rotationVector);


                //Calibration 
                float[] adjustedRotationMatrix = new float[9];
                SensorManager.RemapCoordinateSystem(rotationMatrix, Axis.X,
                                                    Axis.Y, adjustedRotationMatrix);

                //Retrieve calibrated data
                var orientation = new float[3];
                SensorManager.GetOrientation(adjustedRotationMatrix, orientation);

                //Todo: find out what the hell is -57
                _pitch = orientation[1] * -57;
                _roll = orientation[2] * -57;

                ProcessPosition();
                //Log.Debug("ROTATION", $"Pitch: {_pitch}, roll: {_roll}");
            }
        }

        private void ProcessPosition()
        {
            bool exceedsTop = _pitch > SuggestedPitchMax - PitchOffset;
            bool exceedsBot = _pitch < SuggestedPitchMin + PitchOffset;

            if (!gameStarted)
            {
                _activity.UpdateGuideline(exceedsTop, exceedsBot);
                return;
            }

            bool exceedsLeft = _roll < _referencePointRoll - MaxRollDeviaton - RollOffset;
            bool exceedsRight = _roll > _referencePointRoll + MaxRollDeviaton + RollOffset;

            _activity.UpdateGuideline(exceedsTop, exceedsBot, exceedsLeft, exceedsRight);

            if (exceedsTop || exceedsLeft || exceedsRight || exceedsBot)
                _vibration.Start();
            else
                _vibration.Stop();
        }

        public void CapturePosition()
        {
            _referencePointRoll = _roll;
            gameStarted = true;
        }

        public void StartListening()
        {
            _sensorManager.RegisterListener(this, _rotationSensor, SensorDelay.Normal);
        }

        public void StopListening()
        {
            _vibration.Stop();
            _sensorManager.UnregisterListener(this);
        }
    }
}
