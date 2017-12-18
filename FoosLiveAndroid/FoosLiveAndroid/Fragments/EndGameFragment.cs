using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Emgu.CV;
using Emgu.CV.Structure;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Util;
using FoosLiveAndroid.Util.Drawing;
using System;
using System.Threading.Tasks;

namespace FoosLiveAndroid.Fragments
{
    public class EndGameFragment : Fragment
    {
        static readonly new string Tag = typeof(EndGameFragment).Name;
        private static string SpeedFormat;

        private View _view;
        private TextView _team1Name;
        private TextView _team2Name;
        private TextView _teamScore;
        private TextView _durationValue;
        private TextView _avgBallSpeed;
        private TextView _maxBallSpeed;
        private TextView _fastestGoal;
        public ImageView ballHeatMap;
        private ProgressBar _loadingBarHeatMap;

        public static Fragment NewInstance()
        {
            return new EndGameFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_end_game, container, false);
            // If SpeedFormat is not loaded yet, load it
            SpeedFormat = SpeedFormat ?? GetString(Resource.String.speed_format);

            GetReferencesFromLayout();
            _team1Name.Text = MatchInfo.Team1Name;
            _team2Name.Text = MatchInfo.Team2Name;
            _teamScore.Text = String.Format(GetString(Resource.String.score_format_end_game), MatchInfo.Team1Score, MatchInfo.Team2Score);
            _durationValue.Text = MatchInfo.Duration;
            _avgBallSpeed.Text = MatchInfo.AvgSpeed.ToString(SpeedFormat);
            _maxBallSpeed.Text = MatchInfo.MaxSpeed.ToString(SpeedFormat);
            _loadingBarHeatMap.Visibility = ViewStates.Visible;
            ballHeatMap.Post(() =>
            {
                Task.Run(() =>
                {
                Bitmap toDraw = Bitmap.CreateBitmap(ballHeatMap.Width, ballHeatMap.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas();

                canvas.SetBitmap(toDraw);
                HeatmapDrawer.DrawZones(canvas, MatchInfo.Zones);

                Image<Bgr, byte> toBlur = new Image<Bgr, byte>(toDraw);
                CvInvoke.MedianBlur(toBlur, toBlur, PropertiesManager.GetIntProperty("blur_iterations"));

                Activity.RunOnUiThread(() =>
                {
                    ballHeatMap.SetImageBitmap(toBlur.Bitmap);
                    _loadingBarHeatMap.Visibility = ViewStates.Gone;
                });

                });
            });

            // Find the fastest goal
            _fastestGoal.Post(() =>
            {
                // if there are no goals, assign 0 to minDuration
                long minDuration = (MatchInfo.Goals.Count > 0) ? MatchInfo.Goals.Dequeue().Duration : 0;
                
                foreach (var goal in MatchInfo.Goals)
                {
                    if (goal.Duration < minDuration)
                        minDuration = goal.Duration;
                }
                _fastestGoal.Text = minDuration.ToString(SpeedFormat);
            });

            return _view;
        }

        private void GetReferencesFromLayout()
        {
            _team1Name = _view.FindViewById<TextView>(Resource.Id.team1Name);
            _team2Name = _view.FindViewById<TextView>(Resource.Id.team2Name);
            _teamScore = _view.FindViewById<TextView>(Resource.Id.teamScore);
            _durationValue = _view.FindViewById<TextView>(Resource.Id.durationValue);
            _avgBallSpeed = _view.FindViewById<TextView>(Resource.Id.avgBallSpeedValue);
            _maxBallSpeed = _view.FindViewById<TextView>(Resource.Id.maxBallSpeedValue);
            _fastestGoal = _view.FindViewById<TextView>(Resource.Id.fastestGoalValue);
            ballHeatMap = _view.FindViewById<ImageView>(Resource.Id.ballHeatMap);
            _loadingBarHeatMap = _view.FindViewById<ProgressBar>(Resource.Id.loadingBarHeatMap);
        }
    }
}
