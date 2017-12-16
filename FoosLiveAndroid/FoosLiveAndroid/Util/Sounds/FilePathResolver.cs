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
using Android.Content.Res;

namespace FoosLiveAndroid.Util.Sounds
{
    class FilePathResolver
    {
        private const String defaultMarioWin = "Mario Win Sound";
        private const String defaultMarioGoal = "Mario Goal Sound";
        private const String marioGoalSound = "SFX/goal.mp3";
        private const String marioWinSound = "SFX/game_end.wav";

        public static AssetFileDescriptor getFile(Activity activity, String argument)
        {
            System.Console.WriteLine(argument);
            switch(argument)
            {
                case defaultMarioGoal:
                    {
                        return activity.Assets.OpenFd(marioGoalSound);
                    }
                case defaultMarioWin:
                    {
                        return activity.Assets.OpenFd(marioWinSound);
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}