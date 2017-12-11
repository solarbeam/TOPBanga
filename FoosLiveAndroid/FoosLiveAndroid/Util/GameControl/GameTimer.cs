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
using System.Threading.Tasks;

namespace FoosLiveAndroid.Util.GameControl
{
    class GameTimer
    {
        /// <summary>
        /// Defines the current calculated time in miliseconds
        /// </summary>
        public static long Time { get; private set; }

        /// <summary>
        /// Defines whether the timer thread is started
        /// </summary>
        private static bool _threadStarted;
        /// <summary>
        /// Used to lock the Time attribute
        /// </summary>
        private static object _attributeLocker = new object();

        /// <summary>
        /// Defines whether the timer thread needs to be stopped
        /// </summary>
        private bool _stopThread;
        /// <summary>
        /// Defines the function the thread executes
        /// </summary>
        private Action _timer;
        /// <summary>
        /// The task, holding the _timer action
        /// </summary>
        private Task _timerThread;

        public GameTimer( int updateFrequency )
        {
            Time = 0;

            _timer = async () =>
            {
                for (;;)
                {
                    if ( !_stopThread )
                    {
                        await Task.Delay(updateFrequency);

                        // Lock the attribute to ensure thread safety
                        lock(_attributeLocker)
                        {
                            Time += updateFrequency;
                        }
                    }
                    else
                    {
                        _threadStarted = false;
                        _stopThread = false;
                        break;
                    }
                }
            };

            _timerThread = new Task(_timer);
        }

        public bool Start()
        {
            if (!_threadStarted)
            {
                _timerThread.Start();
                _threadStarted = true;
                return true;
            }
            else
                return false;
        }

        public bool Stop()
        {
            if (_threadStarted)
            {
                _stopThread = true;
                return true;
            }
            else
                return false;
        }
    }
}