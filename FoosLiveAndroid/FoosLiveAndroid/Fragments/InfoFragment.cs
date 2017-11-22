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
        public static new string Tag = "InfoFragment";
        private View view;
        private IOnFragmentInteractionListener interactionListener;

        public static Fragment NewInstance()
        {
            return new InfoFragment();
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
            interactionListener.UpdateTitle(GetString(Resource.String.info));
            view = inflater.Inflate(Resource.Layout.fragment_info, container, false);

            return view;
        }

        private void GetReferencesFromLayout()
        {
        }
    }
}
