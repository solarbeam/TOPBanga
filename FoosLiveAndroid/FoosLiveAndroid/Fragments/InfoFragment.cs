using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;

namespace FoosLiveAndroid.Fragments
{
    public class InfoFragment : Fragment
    {
        static readonly new string Tag = typeof(InfoFragment).Name;

        private View _view;
        private IOnFragmentInteractionListener _interactionListener;

        public static Fragment NewInstance()
        {
            return new InfoFragment();
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
            _interactionListener.UpdateTitle(GetString(Resource.String.info));
            _view = inflater.Inflate(Resource.Layout.fragment_info, container, false);

            return _view;
        }

        private void GetReferencesFromLayout()
        {
        }
    }
}
