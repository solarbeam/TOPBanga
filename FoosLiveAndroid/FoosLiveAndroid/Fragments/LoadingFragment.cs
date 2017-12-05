
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Util.Database;

namespace FoosLiveAndroid.Fragments
{
    public class LoadingFragment : Fragment
    {
        static readonly new string Tag = typeof(InfoFragment).Name;
        private IOnFragmentInteractionListener _interactionListener;
        private View _view;
        private TextView _errorLabel;
        private ProgressBar _progressBar;

        public ProgressBar ProgressBar { get => _progressBar; set => _progressBar = value; }

        public static LoadingFragment NewInstance()
        {
            return new LoadingFragment();
        }

        public override void OnAttach(Context context)
        {
            try
            {
                _interactionListener = (IOnFragmentInteractionListener)context;
            }
            catch (InvalidCastException)
            {
                Log.Error(Tag, "IOnFragmentInteractionListener not implemented in parent activity");
                throw;
            }
            base.OnAttach(context);
        }

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var _historyList = await DatabaseManager.GetHistory();

            //Todo: replace temporary solution
            DatabaseManager.tempDataStorage = _historyList;

            Log.Debug("LIST SIZE", _historyList.Count.ToString());
            // If no data were retrieved, display error and ignore list initialisation
            if (_historyList == null)
            {
                DisplayError();
                return;
            }

            // If there are no records, display message and ignore list initialisation
            if (_historyList.Count == 0)
            {
                _errorLabel.Text = GetString(Resource.String.history_empty);
                DisplayError();
                return;
            }

            // load data fragm
            _interactionListener.LoadFragment(FragmentId.History, false);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_loading, container, false);
            _errorLabel = _view.FindViewById<TextView>(Resource.Id.errorLabel);
            _progressBar = _view.FindViewById<ProgressBar>(Resource.Id.loadingBar);
            return _view;
        }

        /// <summary>
        /// Displays the loading error.
        /// </summary>
        void DisplayError()
        {
            _progressBar.Visibility = ViewStates.Gone;
            _errorLabel.Visibility = ViewStates.Visible;
        }
    }
}
