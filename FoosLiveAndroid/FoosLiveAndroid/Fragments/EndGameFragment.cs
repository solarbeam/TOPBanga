using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Util.Drawing;
using System;
using System.Threading.Tasks;

namespace FoosLiveAndroid.Fragments
{
    public class EndGameFragment : Fragment
    {
        private static readonly string SpeedFormat = "0.00 cm/s";
        static readonly new string Tag = typeof(InfoFragment).Name;

        private View _view;
        private TextView _team1Name;
        private TextView _team2Name;
        private TextView _teamScore;
        private TextView _durationValue;
        private TextView _avgBallSpeed;
        private TextView _maxBallSpeed;
        private TextView _fastestGoal;
        public ImageView ballHeatMap;

        public static Fragment NewInstance()
        {
            return new EndGameFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_end_game, container, false);

            GetReferencesFromLayout();

            _team1Name.Text = MatchInfo.Team1Name;
            _team2Name.Text = MatchInfo.Team2Name;
            _teamScore.Text = GetString(Resource.String.score_format_end_game, MatchInfo.Team1Score, MatchInfo.Team2Score);
            _durationValue.Text = MatchInfo.Duration;
            _avgBallSpeed.Text = MatchInfo.AvgSpeed.ToString(SpeedFormat);
            _maxBallSpeed.Text = MatchInfo.MaxSpeed.ToString(SpeedFormat);

            ballHeatMap.Post(() =>
            {
                Bitmap toDraw = Bitmap.CreateBitmap(ballHeatMap.Width, ballHeatMap.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas();
                canvas.SetBitmap(toDraw);
                HeatmapDrawer.DrawZones(canvas, MatchInfo.Zones);
                ballHeatMap.SetImageBitmap(toDraw);
            });

            // Find the fastest goal
            _fastestGoal.Post(() =>
            {
                long min = 999999999;
                foreach (var goal in MatchInfo.Goals)
                {
                    if (goal.Duration < min)
                        min = goal.Duration;
                }

                _fastestGoal.Text = Math.Round(min * 0.001, 2).ToString("0.00 s");
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
        }
    }
}
