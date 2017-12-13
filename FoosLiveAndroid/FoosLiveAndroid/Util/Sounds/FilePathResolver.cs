using System;
using Android.Content;
using Android.Content.Res;

namespace FoosLiveAndroid.Util.Sounds
{
    class FilePathResolver
    {
        private const String defaultMarioWin = "defaultMarioWin";
        private const String defaultMarioGoal = "defaultMarioGoal";
        private const String marioGoalSound = "SFX/goal.mp3";
        private const String marioWinSound = "SFX/game_end.wav";

        public static AssetFileDescriptor GetFile(Context context, String argument)
        {
            switch(argument)
            {
                case defaultMarioGoal:
                    {
                        return context.Assets.OpenFd(marioGoalSound);
                    }
                case defaultMarioWin:
                    {
                        return context.Assets.OpenFd(marioWinSound);
                    }
            }
            return null;
        }
    }
}