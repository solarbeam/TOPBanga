using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;

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
        private TextView _avgBallSpeedValue;
        private TextView _maxBallSpeedValue;

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
            _avgBallSpeedValue.Text = MatchInfo.AvgSpeed.ToString(SpeedFormat);
            _maxBallSpeedValue.Text = MatchInfo.MaxSpeed.ToString(SpeedFormat);
            return _view;
        }

        private void GetReferencesFromLayout()
        {
            _team1Name = _view.FindViewById<TextView>(Resource.Id.team1Name);
            _team2Name = _view.FindViewById<TextView>(Resource.Id.team2Name);
            _teamScore = _view.FindViewById<TextView>(Resource.Id.teamScore);
            _durationValue = _view.FindViewById<TextView>(Resource.Id.durationValue);
            _avgBallSpeedValue = _view.FindViewById<TextView>(Resource.Id.avgBallSpeedValue);
            _maxBallSpeedValue = _view.FindViewById<TextView>(Resource.Id.maxBallSpeedValue);
        }
    }
}
