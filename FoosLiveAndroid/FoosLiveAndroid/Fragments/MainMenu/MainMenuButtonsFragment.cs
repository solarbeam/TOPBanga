using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments
{
    public class MainMenuButtonsFragment : Fragment
    {
        public new string Tag = "MainMenuButtonsFragment";
        private View view;

        private Button startButton;
        private Button settingsButton;
        private Button infoButton;

        private IOnFragmentInteractionListener interactionListener;

        public static Fragment NewInstance()
        {
            return new MainMenuButtonsFragment();
        }

        public override void OnAttach(Context context)
        {
            try
            {
                interactionListener = (IOnFragmentInteractionListener) context;
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
            interactionListener.UpdateTitle(GetString(Resource.String.main_menu));
            view = inflater.Inflate(Resource.Layout.main_menu_items, container, false);
            //Main menu buttons
            GetReferencesFromLayout();
            startButton.Click += delegate
            {
                ((MainMenuFragment)ParentFragment).LoadChildFragment(FragmentId.Mode_menu);
            };

            settingsButton.Click += delegate
            {   
                interactionListener.LoadFragment(FragmentId.Settings);
            };

            infoButton.Click += delegate
            {
                interactionListener.LoadFragment(FragmentId.Info);
            };

            return view;
        }

        private void GetReferencesFromLayout()
        {
            startButton = view.FindViewById<Button>(Resource.Id.startButton);
            settingsButton = view.FindViewById<Button>(Resource.Id.settingsButton);
            infoButton = view.FindViewById<Button>(Resource.Id.infoButton);
        }
    }
}
