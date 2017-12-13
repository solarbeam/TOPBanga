using System;
using System.Timers;

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

        /// <summary>
        /// The default constructor for the GameTimer class
        /// </summary>
        /// <param name="interval">The interval between increments</param>
        public GameTimer(int interval)
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = interval;
            _toAdd = interval;
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
        public bool Start()
        {
            if (!_timer.Enabled)
            {
                Time = 0;
                _timer.Enabled = true;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        /// <returns>True if the timer is stopped succesfully. False otherwise</returns>
        public bool Stop()
        {
            if (_timer.Enabled)
            {
                _timer.Enabled = false;
                return true;
            }
            else
                return false;
        }
    }
}