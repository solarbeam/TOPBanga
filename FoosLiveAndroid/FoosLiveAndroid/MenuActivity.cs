using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Fragments;
using Android.Content.PM;

namespace FoosLiveAndroid
{
    [Activity(Label = "Fooslive", MainLauncher = true, Icon = "@mipmap/icon_round",
              ScreenOrientation = ScreenOrientation.Portrait)]
    public class MenuActivity : AppCompatActivity, IOnFragmentInteractionListener
    {
        public static string Tag = "MenuActivity";

        // Todo: replace with fragmentmanager 
        private Fragment previousFragment;
        private Fragment fragment;

        private TextView toolbarTitle;
        private Android.Support.V7.Widget.Toolbar toolbar;

        /// <summary>
        /// Called whenever the view is created
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_menu);
            GetReferencesFromLayout();
            SetSupportActionBar(toolbar);
            // hide default top bar title
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            // load initial fragment
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.menu_content, fragment = MainMenuFragment.NewInstance())
                           .Commit();
        }

        /// <summary>
        /// Set the instances according to the layout, defined in Resources/layout/activity_game.axml
        /// </summary>
        private void GetReferencesFromLayout()
        {
            toolbarTitle = FindViewById<TextView>(Resource.Id.toolbarTitle);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        }

        /// <summary>
        /// Called when back button is pressed
        /// </summary>
        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount > 0)
            {
                fragment = previousFragment;
                FragmentManager.PopBackStack();
            }
            else if (fragment is MainMenuFragment && 
                     fragment.ChildFragmentManager.BackStackEntryCount > 0) 
            {
                fragment.ChildFragmentManager.PopBackStack();
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
            previousFragment = fragment;
            fragment = null;
            switch (id)
            {
                case FragmentId.Main_menu:
                    fragment = MainMenuFragment.NewInstance();
                    break;
                case FragmentId.Settings:
                    fragment = SettingsFragment.NewInstance();
                    break;
                case FragmentId.Info:
                    fragment = InfoFragment.NewInstance();
                    break;
                default:
                    Log.Error(Tag, $"SwitchFragment unknown ID: {id}");
                    break;
            }

            if (fragment != null)
            {
                FragmentManager.BeginTransaction()
                               .Replace(Resource.Id.menu_content, fragment)
                               .AddToBackStack(fragment.Tag)
                               .Commit();
            }
        }
        /// <summary>
        /// Update top actionbar title
        /// </summary>
        public void UpdateTitle(string title)
        {
            toolbarTitle.Text = title;
        }
    }
}