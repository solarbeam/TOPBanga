using Android.App;
using Android.OS;
using Android.Views;

namespace FoosLiveAndroid.Fragments
{
    public class SettingsFragment : Fragment
    {
        private View view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // return inflater.Inflate(Resource.Layout.fragment_settings, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}
