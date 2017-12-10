using Android.App;
using Android.OS;
using Android.Views;

namespace FoosLiveAndroid.Fragments
{
    public class EndGameFragment : Fragment
    {
        static readonly new string Tag = typeof(InfoFragment).Name;

        private View _view;

        public static Fragment NewInstance()
        {
            return new EndGameFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_end_game, container, false);

            return _view;
        }

        private void GetReferencesFromLayout()
        {
        }
    }
}
