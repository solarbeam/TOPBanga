using Android.Content;
using Android.Media;
using FoosLiveAndroid.Util.Interface;

namespace FoosLiveAndroid.Util.Sounds
{
    public class PlayerOGG : IAlert
    {
        private readonly MediaPlayer _player;

        //Todo: paths from cfg

        public PlayerOGG(Context context, Android.Net.Uri path)
        {
            _player = MediaPlayer.Create(context, path); 
        }

        public void Play()
        {
            _player.Start();
        }
    }
}
