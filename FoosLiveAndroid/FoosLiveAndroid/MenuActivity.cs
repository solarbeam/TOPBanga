using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Fragments;
using Android.Content.PM;
using FoosLiveAndroid.Fragments.MainMenu;
using FoosLiveAndroid.Util;

namespace FoosLiveAndroid
{
    [Activity(Label = "Fooslive", Icon = "@mipmap/icon_round",
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class MenuActivity : AppCompatActivity, IOnFragmentInteractionListener
    {
        static readonly string Tag = typeof(MenuActivity).Name;

        // Todo: replace with fragmentmanager 
        private Fragment _previousFragment;
        private Fragment _fragment;

        private TextView _toolbarTitle;
        private Android.Support.V7.Widget.Toolbar _toolbar;

        /// <summary>
        /// Called whenever the view is created
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_menu);
            GetReferencesFromLayout();
            SetSupportActionBar(_toolbar);
            // hide default top bar title
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            // load initial fragment
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.menu_content, _fragment = MainMenuFragment.NewInstance())
                           .Commit();
        }

        /// <summary>
        /// Set the instances according to the layout, defined in Resources/layout/activity_game.axml
        /// </summary>
        private void GetReferencesFromLayout()
        {
            _toolbarTitle = FindViewById<TextView>(Resource.Id.toolbarTitle);
            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        }

        /// <summary>
        /// Called when back button is pressed
        /// </summary>
        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount > 0)
            {
                _fragment = _previousFragment;
                FragmentManager.PopBackStack();
            }
            else if (_fragment is MainMenuFragment && 
                     _fragment.ChildFragmentManager.BackStackEntryCount > 0) 
            {
                _fragment.ChildFragmentManager.PopBackStack();
            }
            else
            {
                Finish();
            }
        }

        /// <summary>
        /// Load the fragment on top of activity view
        /// </summary>
        public void LoadFragment(FragmentId id)
        {
            _previousFragment = _fragment;
            _fragment = null;
            switch (id)
            {
                case FragmentId.MainMenu:
                    _fragment = MainMenuFragment.NewInstance();
                    break;
                case FragmentId.Settings:
                    _fragment = SettingsFragment.NewInstance();
                    break;
                case FragmentId.Info:
                    _fragment = InfoFragment.NewInstance();
                    break;
                default:
                    Log.Error(Tag, $"SwitchFragment unknown ID: {id}");
                    break;
            }

            if (_fragment != null)
            {
                FragmentManager.BeginTransaction()
                               .Replace(Resource.Id.menu_content, _fragment)
                               .AddToBackStack(_fragment.Tag)
                               .Commit();
            }
        }
        /// <summary>
        /// Update top actionbar title
        /// </summary>
        public void UpdateTitle(string title)
        {
            _toolbarTitle.Text = title;
        }
    }
}
