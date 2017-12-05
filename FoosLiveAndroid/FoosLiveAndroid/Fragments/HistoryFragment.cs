using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
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
            //Todo: change temp solution
            var adapter = new HistoryListAdapter(DatabaseManager.tempDataStorage);

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
