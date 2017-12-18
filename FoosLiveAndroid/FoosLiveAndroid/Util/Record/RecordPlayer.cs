using Android.Content;
using Android.Media;

namespace FoosLiveAndroid.Util.Record
{
    public sealed class RecordPlayer : MediaPlayer, MediaPlayer.IOnPreparedListener,
    MediaPlayer.IOnCompletionListener
    {
        // false on init, true on creation
        public bool Disposed;

        private GameActivity _activity;

        public RecordPlayer(Context context)
        {
            _activity = (GameActivity)context;
            Disposed = false;
            SetDataSource(context, _activity.Intent.Data);
            SetSurface(_activity.SurfaceManager.Surface);
            Prepare();
            SetOnPreparedListener(this);
            SetOnCompletionListener(this);
        }

        /// <summary>
        /// Called whenever the mediaplayer is ready to be started
        /// </summary>
        /// <param name="mediaPlayer">The MediaPlayer instance, which called this function</param>
        public void OnCompletion(MediaPlayer mediaPlayer)
        {
            if (!Disposed)
            {
                mediaPlayer.Release();
                mediaPlayer.Dispose();
                Disposed = true;
            }
            _activity.ShowEndGameScreen();
        }
        public override void Release()
        {
            if (Disposed) return;
            base.Release();
            Dispose();
            Disposed = true;
        }
        public void OnPrepared(MediaPlayer mediaPlayer)
        {
            // Load video
            mediaPlayer.Start();

            // We only need the frames from the video, so mute the sound
            mediaPlayer.SetVolume(0, 0);

            // Pause the video to let the user choose an Hsv value
            mediaPlayer.Pause();
        }
    }
}
