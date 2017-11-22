using System;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments
{
    public class MainMenuFragment : Fragment
    {
        public static new string Tag = "MainMenuFragment";
        private View view;

        public static Fragment NewInstance()
        {
            return new MainMenuFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_main_menu, container, false);

            LoadChildFragment(FragmentId.Main_menu);

            return view;
        }

        public void LoadChildFragment(FragmentId id) {
            Fragment childFragment = null;
            FragmentTransaction transaction = ChildFragmentManager.BeginTransaction();
            switch(id) 
            {
                case FragmentId.Main_menu:
                    childFragment = MainMenuButtonsFragment.NewInstance();
                    break;
                case FragmentId.Mode_menu:
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
