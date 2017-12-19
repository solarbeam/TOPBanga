using Android.Content;
using Android.Content.Res;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Util.Sounds
{
    static class FilePathResolver
    {
        private static readonly string DefaultMarioWin = SoundAsset.WinMario.ToString();
        private static readonly string DefaultMarioGoal = SoundAsset.GoalMario.ToString();
        private static readonly string DefaultBingSound = SoundAsset.BingSound.ToString();
        private static readonly string DefaultCrowdCheerSound = SoundAsset.CrowdCheer.ToString();

        private const string MarioGoalSound = "SFX/goal.mp3";
        private const string MarioWinSound = "SFX/game_end.wav";
        private const string BingSound = "SFX/ping-bing.wav";
        private const string CrowdCheer = "SFX/sound.ogg";

        public static AssetFileDescriptor GetFile(Context context, string argument)
        {
            // Todo: make it look better
            if (argument.Equals(DefaultMarioGoal))
            {
                return context.Assets.OpenFd(MarioGoalSound);
            }
            if (argument.Equals(DefaultMarioWin))
            {
                return context.Assets.OpenFd(MarioWinSound);
            }
            if (argument.Equals(DefaultBingSound))
            {
                return context.Assets.OpenFd(BingSound);
            }
            if (argument.Equals(DefaultCrowdCheerSound))
            {
                return context.Assets.OpenFd(CrowdCheer);
            }

            return null;
        }
    }
}