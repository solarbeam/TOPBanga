using System;
using Android.Content;
using Android.Content.Res;

namespace FoosLiveAndroid.Util.Sounds
{
    static class FilePathResolver
    {
        private const string DefaultMarioWin = "defaultMarioWin";
        private const string DefaultMarioGoal = "defaultMarioGoal";
        private const string MarioGoalSound = "SFX/goal.mp3";
        private const string MarioWinSound = "SFX/game_end.wav";

        public static AssetFileDescriptor GetFile(Context context, String argument)
        {
            switch(argument)
            {
                case DefaultMarioGoal:
                    {
                        return context.Assets.OpenFd(MarioGoalSound);
                    }
                case DefaultMarioWin:
                    {
                        return context.Assets.OpenFd(MarioWinSound);
                    }
            }
            return null;
        }
    }
}