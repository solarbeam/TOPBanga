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
using System.Timers;

namespace FoosLiveAndroid.Util.GameControl
{
    class GameTimer
    {
        public static long Time { get; private set; }

        public EventHandler<EventArgs> OnUpdated;

        private Timer _timer;
        private object _locker = new object();
        private int _toAdd;

        public GameTimer(int interval)
        {
            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Interval = interval;
            _toAdd = interval;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            lock(_locker)
            {
                Time += _toAdd;
            }
            OnUpdated(this, EventArgs.Empty);
        }

        public bool Start()
        {
            if (_timer.Enabled == false)
            {
                _timer.Enabled = true;
                return true;
            }
            else
                return false;
        }

        public bool Stop()
        {
            if (_timer.Enabled = true)
            {
                _timer.Enabled = false;
                return true;
            }
            else
                return false;
        }
    }
}