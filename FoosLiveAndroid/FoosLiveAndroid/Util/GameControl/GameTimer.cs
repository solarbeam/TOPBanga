using System;
using System.Timers;
using Android.Content.Res;

namespace FoosLiveAndroid.Util.GameControl
{
    class GameTimer
    {
        /// <summary>
        /// Defines time calculated in miliseconds
        /// </summary>
        public static long Time { get; private set; }

        /// <summary>
        /// Fires whenever the Time attribute updates
        /// </summary>
        // Todo: redundant variable
        public EventHandler<EventArgs> OnUpdated;

        /// <summary>
        /// Defines the timer object, which adds to the Time attribute
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// Defines the object, which ensures thread safety for the Time attribute
        /// </summary>
        private object _locker = new object();
        /// <summary>
        /// Defines how much we increment the Time attribute
        /// </summary>
        private int _toAdd;

        private readonly string _timeFormat;

        /// <summary>
        /// The default constructor for the GameTimer class
        /// </summary>
        /// <param name="interval">The interval between increments</param>
        /// <param name="resources">Static resources reference</param>
        public GameTimer(int interval, Resources resources)
        {
            _timer = new Timer();
            _timer.Elapsed += OnTimedEvent;
            _timer.Interval = interval;
            _toAdd = interval;
            _timeFormat = resources.GetString(Resource.String.timer_format);
        }

        /// <summary>
        /// A function, which is called by the _timer attribute
        /// </summary>
        /// <param name="source">The caller</param>
        /// <param name="e">Event arguments, associated with the call</param>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            // Lock the attribute to ensure thread safety
            lock(_locker)
            {
                Time += _toAdd;
            }
            // Fire the OnUpdated event
            OnUpdated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        /// <returns>True if started succesfully. False otherwise</returns>
        public void Start()
        {
            if (_timer.Enabled) return;
            Time = 0;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        /// <returns>True if the timer is stopped succesfully. False otherwise</returns>
        public void Stop()
        {
            _timer.Enabled = false;
        }

        public string GetFormattedTime()
        {
            return TimeSpan.FromMilliseconds(Time).ToString(@"mm\:ss");
        }
    }
}