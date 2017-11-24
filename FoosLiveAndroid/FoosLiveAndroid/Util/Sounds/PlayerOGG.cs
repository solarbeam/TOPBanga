using Android.Content;
using Android.Media;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Sounds
{
    public class PlayerOGG : IAlert
    {
        readonly MediaPlayer player;

        //Todo: paths from cfg

        public PlayerOGG(Context context, Android.Net.Uri path)
        {
            player = MediaPlayer.Create(context, path); 
        }

        public void Play()
        {
            player.Start();
        }
    }
}
