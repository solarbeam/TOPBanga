using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using FoosLiveAndroid.Model.Interface;
using FoosLiveAndroid.Util.Database;

namespace FoosLiveAndroid.Fragments
{
    public class HistoryFragment : Fragment
    {
        static readonly new string Tag = typeof(InfoFragment).Name;

        private View _view;
        private IOnFragmentInteractionListener _interactionListener;
        private RecyclerView _historyRecyclerView;

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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _interactionListener.UpdateTitle(GetString(Resource.String.history));
            _view = inflater.Inflate(Resource.Layout.fragment_history, container, false);

            GetReferencesFromLayout();

            var layoutManager = new LinearLayoutManager(Activity);
            _historyRecyclerView.SetLayoutManager(layoutManager);

            List<IHistory> historyList = DatabaseManager.GetHistory();

            // Instantiate the adapter and pass in its data source:
            HistoryListAdapter adapter = new HistoryListAdapter(historyList);

            // Plug the adapter into the RecyclerView:
            _historyRecyclerView.SetAdapter(adapter);

            return _view;
        }

        private void GetReferencesFromLayout()
        {
            _historyRecyclerView = _view.FindViewById<RecyclerView>(Resource.Id.historyRecyclerView);
        }
    }
}
