using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Fragments.Interface;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments.MainMenu
{
    public class MainMenuButtonsFragment : Fragment
    {
        static readonly new string Tag = typeof(MainMenuButtonsFragment).Name;
        private View _view;

        private Button _startButton;
        private Button _settingsButton;
        private Button _infoButton;
        private Button _historyButton;


        private IOnFragmentInteractionListener _interactionListener;

        public static Fragment NewInstance()
        {
            return new MainMenuButtonsFragment();
        }

        public override void OnAttach(Context context)
        {
            try
            {
                _interactionListener = (IOnFragmentInteractionListener) context;
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
            _interactionListener.UpdateTitle(GetString(Resource.String.main_menu));
            _view = inflater.Inflate(Resource.Layout.main_menu_items, container, false);
            //Main menu buttons
            GetReferencesFromLayout();

            _startButton.Click += delegate
            {
                ((MainMenuFragment)ParentFragment).LoadChildFragment(FragmentId.ModeMenu);
            };

            _settingsButton.Click += delegate
            {   
                _interactionListener.LoadFragment(FragmentId.Settings);
            };

            _historyButton.Click += delegate
            {
                _interactionListener.LoadFragment(FragmentId.History);
            };

            _infoButton.Click += delegate
            {
                _interactionListener.LoadFragment(FragmentId.Info);
            };

            return _view;
        }

        private void GetReferencesFromLayout()
        {
            _startButton = _view.FindViewById<Button>(Resource.Id.startButton);
            _settingsButton = _view.FindViewById<Button>(Resource.Id.settingsButton);
            _historyButton = _view.FindViewById<Button>(Resource.Id.historyButton);
            _infoButton = _view.FindViewById<Button>(Resource.Id.infoButton);

        }
    }
}
