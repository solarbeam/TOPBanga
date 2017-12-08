using System;
using Android.Hardware;
using Android.Runtime;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Sensors
{
    public class PositionManager : Java.Lang.Object, ISensorEventListener, IPositionManager
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
        // 0 - top, 1 - bot
        private bool[] _exceedsPitch;
        // 0 - left, 1 - right
        private bool?[] _exceedsRoll;


        private GameActivity _activity;

        private bool gameStarted = false;

        public PositionManager(GameActivity activity, SensorManager sensorManager, Vibration vibration)
        {
            _activity = activity;
            _rotationSensor = sensorManager.GetDefaultSensor(SensorType.RotationVector);
            _vibration = vibration;
            _sensorManager = sensorManager;
        }

        /// <summary>
        /// Called on change of sensors accuracy. Captures accuracy status.
        /// </summary>
        /// <param name="sensor">Sensor</param>
        /// <param name="accuracy">Accuracy</param>
        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            _lastAccuracy = accuracy;
        }

        /// <summary>
        /// Called on change of sensors values. Analyses value changes.
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

                // Extract raw data
                for (int i = 0; i < rotationVector.Length; i++)
                    rotationVector[i] = e.Values[i];

                // Parse raw data
                SensorManager.GetRotationMatrixFromVector(rotationMatrix, rotationVector);

                // Calibration 
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

        /// <summary>
        /// Processes the position: analyses pitch and roll data
        /// </summary>
        private void ProcessPosition()
        {
            _exceedsPitch = new bool[2];
            _exceedsPitch[0] = _pitch > SuggestedPitchMax - PitchOffset; // top
            _exceedsPitch[1] = _pitch < SuggestedPitchMin + PitchOffset; // bot
            if (!gameStarted)
            {
                _activity.UpdateGuideline(_exceedsPitch);
                return;
            }

            _exceedsRoll = new bool?[2];
            _exceedsRoll[0] = _roll < _referencePointRoll - MaxRollDeviaton - RollOffset; //left
            _exceedsRoll[1] = _roll > _referencePointRoll + MaxRollDeviaton + RollOffset; //right
             
            _activity.UpdateGuideline(_exceedsPitch, _exceedsRoll);

            if (_exceedsPitch[0] || _exceedsPitch[1] || _exceedsRoll[0].Value || _exceedsRoll[1].Value)
                _vibration?.Start();
            else
                _vibration?.Stop();
        }

        /// <summary>
        /// Captures the position at the time game was started
        /// </summary>
        public void CapturePosition()
        {
            _referencePointRoll = _roll;
            gameStarted = true;
        }

        /// <summary>
        /// Make rotation sensors active
        /// </summary>
        public void StartListening()
        {
            _sensorManager.RegisterListener(this, _rotationSensor, SensorDelay.Normal);
        }

        /// <summary>
        /// Make rotation sensors inactive
        /// </summary>
        public void StopListening()
        {
            _vibration?.Stop();
            _sensorManager.UnregisterListener(this);
        }
    }
}
