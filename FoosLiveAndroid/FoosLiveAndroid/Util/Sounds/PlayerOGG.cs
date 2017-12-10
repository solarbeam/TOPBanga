using Android.Content;
using Android.Content.Res;
using Android.Media;
using FoosLiveAndroid.Util.Interface;
using Java.IO;

namespace FoosLiveAndroid.Util.Sounds
{
    public class PlayerOGG : IAlert
    {
        private readonly MediaPlayer _player;

        //Todo: paths from cfg

        public PlayerOGG(Context context, AssetFileDescriptor descriptor )
        {
            _player = new MediaPlayer();
            _player.SetVolume(100, 100);
            _player.SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
            _player.Prepare();
        }

        public void Play()
        {
            _player.Start();
        }
    }
}
