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

namespace FoosLiveAndroid.Util.Drawing
{
    class TextEffects
    {
        private static readonly int SlidingTextDelay = PropertiesManager.GetIntProperty("sliding_text_delay");

        private static bool _textThreadStarted;
        /// <summary>
        /// Defines a sliding text effect for a given string of text
        /// </summary>
        /// <param name="text">The text, to which the effect will be applied</param>
        public static void SlideText(string text, Activity activity, TextView textView)
        {
            if (_textThreadStarted)
                return;

            _textThreadStarted = true;

            activity.RunOnUiThread(async () =>
            {
                var temp = text;
                var tempView = new StringBuilder(temp.Length);

                for (var i = 0; i < textView.Length(); i++)
                {
                    tempView.Append(' ');
                }
                textView.Text = tempView.ToString();

                for (var i = 0; i < tempView.Length * 3; i++)
                {
                    tempView.Remove(0, 1);
                    tempView.Append(i < temp.Length ? temp[i] : ' ');

                    textView.Text = tempView.ToString();
                    await Task.Delay(SlidingTextDelay);
                }

                _textThreadStarted = false;
            });
        }
    }
}