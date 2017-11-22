using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;

namespace FoosLiveAndroid.Fragments
{
    public class SettingsFragment : Fragment
    {
        public static new string Tag = "SettingsFragment";

        private View view;
        private IOnFragmentInteractionListener interactionListener;

        public static Fragment NewInstance()
        {
            return new SettingsFragment();
        }

        public override void OnAttach(Context context)
        {
            try
            {
                interactionListener = (IOnFragmentInteractionListener)context;
            }
            catch (InvalidCastException e)
            {
                Log.Error(Tag, "IOnFragmentInteractionListener not implemented in parent activity");
                throw e;
            }

            base.OnAttach(context);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            interactionListener.UpdateTitle(GetString(Resource.String.settings));
            view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);

            return view;
        }

        private void GetReferencesFromLayout()
        {
        }

    }
}
