using Android.Content;
using Android.Media;

namespace FoosLiveAndroid.Util
{
    public class RecordPlayer : MediaPlayer, MediaPlayer.IOnPreparedListener,
    MediaPlayer.IOnCompletionListener
    {
        // false on init, true on creation
        public bool disposed;

        private GameActivity activity;

        public RecordPlayer(Context context)
        {
            activity = (GameActivity)context;
            disposed = false;
            SetDataSource(context, activity.Intent.Data);
            SetSurface(activity._surfaceManager.Surface);
            Prepare();
            SetOnPreparedListener(this);
            SetOnCompletionListener(this);
        }

        /// <summary>
        /// Called whenever the mediaplayer is ready to be started
        /// </summary>
        /// <param name="mp">The MediaPlayer instance, which called this function</param>
        public void OnCompletion(MediaPlayer mediaPlayer)
        {
            if (!disposed)
            {
                mediaPlayer.Release();
                mediaPlayer.Dispose();
                disposed = true;
            }
            activity.ShowEndGameScreen();
        }
        public override void Release()
        {
            base.Release();
            if (!disposed)
            {
                Dispose();
                disposed = true;
            }
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
