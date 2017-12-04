using Android.App;
using Android.OS;
using Android.Views;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments.MainMenu
{
    public class MainMenuFragment : Fragment
    {
        static readonly new string Tag = typeof(MainMenuFragment).Name;
        private View _view;

        public static Fragment NewInstance()
        {
            return new MainMenuFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_main_menu, container, false);

            LoadChildFragment(FragmentId.MainMenu);

            return _view;
        }

        public void LoadChildFragment(FragmentId id) {
            Fragment childFragment = null;
            var transaction = ChildFragmentManager.BeginTransaction();
            switch(id) 
            {
                case FragmentId.MainMenu:
                    childFragment = MainMenuButtonsFragment.NewInstance();
                    break;
                case FragmentId.ModeMenu:
                    childFragment = ModeMenuButtonsFragment.NewInstance();
                    transaction.AddToBackStack(childFragment.Tag);
                    break;
            }

            if (childFragment != null)
                transaction.Replace(Resource.Id.menu_items, childFragment)
                           .Commit();
        }
    }
}
