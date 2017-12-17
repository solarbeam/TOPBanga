using System;
using Android.Content;
using Android.Content.Res;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Util.Sounds
{
    static class FilePathResolver
    {
        private static readonly string DefaultMarioWin = SoundAsset.WinMario.ToString();
        private static readonly string DefaultMarioGoal = SoundAsset.GoalMario.ToString();

        private const string MarioGoalSound = "SFX/goal.mp3";
        private const string MarioWinSound = "SFX/game_end.wav";

        public static AssetFileDescriptor GetFile(Context context, String argument)
        {
            System.Console.WriteLine(argument);

            if (argument.Equals(DefaultMarioGoal))
            {
                return context.Assets.OpenFd(MarioGoalSound);
            }
            else
                if (argument.Equals(DefaultMarioWin))
            {
                return context.Assets.OpenFd(MarioWinSound);
            }
            else
                return null;
        }
    }
}