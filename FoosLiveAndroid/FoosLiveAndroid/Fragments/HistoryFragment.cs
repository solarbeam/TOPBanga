using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Util.Database;
using FoosLiveAndroid.Fragments.Interface;
using FoosLiveAndroid.Model;
using System.Threading.Tasks;
using FoosLiveAndroid.Model.Interface;
using System.Collections.Generic;

namespace FoosLiveAndroid.Fragments
{
    public class HistoryFragment : Fragment
    {
        static readonly new string Tag = typeof(HistoryFragment).Name;

        private TextView _loadingStatusLabel;
        private ProgressBar _progressBar;
        private RelativeLayout _loadingLayout;
        private View _view;
        private IOnFragmentInteractionListener _interactionListener;
        private RecyclerView _historyRecyclerView;
        private LoadingStatus _loadingStatus = LoadingStatus.Unknown;
        private Task<List<IHistory>> historyTask;

        public static Fragment NewInstance()
        {
            return new HistoryFragment();
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

        public async override void OnCreate(Bundle savedInstanceState)
        {
            if (historyTask == null)
                historyTask = DatabaseManager.GetHistory();
            base.OnCreate(savedInstanceState);
            var _historyList = await historyTask;

            // If no data was retrieved, display error and ignore list initialisation
            if (_historyList == null)
            {
                _loadingStatus = LoadingStatus.No_connection;
                return;
            }

            // If there are no records, display message and ignore list initialisation
            if (_historyList.Count == 0)
            {   
                _loadingStatus = LoadingStatus.Empty_list;
                return;
            }

            _loadingStatus = LoadingStatus.Success;
            // Hides loading layout and shows history list
            _loadingLayout.Visibility = ViewStates.Gone;
            _historyRecyclerView.Visibility = ViewStates.Visible;
            // Creates adapter for recycler view
            var adapter = new HistoryListAdapter(_historyList);
            adapter.NotifyDataSetChanged();
            // Plug the adapter into the RecyclerView:
            _historyRecyclerView.SetAdapter(adapter);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if(historyTask == null)
                historyTask = DatabaseManager.GetHistory();

            _interactionListener.UpdateTitle(GetString(Resource.String.history));
            _view = inflater.Inflate(Resource.Layout.fragment_history, container, false);
            GetReferencesFromLayout();

            var layoutManager = new LinearLayoutManager(Activity);
            _historyRecyclerView.SetLayoutManager(layoutManager);

            _view.Post(async () =>
            {
                await historyTask;

                if (_loadingStatus == LoadingStatus.Success) return;

                if (_loadingStatus == LoadingStatus.Empty_list)
                {
                    _loadingStatusLabel.Text = GetString(Resource.String.history_empty);
                    ShowError();
                    return;
                }
                if (_loadingStatus == LoadingStatus.No_connection)
                    ShowError();
            });

            return _view;
        }

        private void GetReferencesFromLayout()
        {
            _loadingLayout = _view.FindViewById<RelativeLayout>(Resource.Id.loadingLayout);
            _loadingStatusLabel = _view.FindViewById<TextView>(Resource.Id.loadingStatusLabel);
            _progressBar = _view.FindViewById<ProgressBar>(Resource.Id.loadingBar);
            _historyRecyclerView = _view.FindViewById<RecyclerView>(Resource.Id.historyRecyclerView);
        }

        private void ShowError() 
        {
            // Hide progress bar
            _progressBar.Post(() => _progressBar.Visibility = ViewStates.Gone);
            // Show error message
            _loadingStatusLabel.Post(() => _loadingStatusLabel.Visibility = ViewStates.Visible);
        }
    }
}
