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
        public const string Tag = "MenuActivity";

        // Todo: replace with fragmentmanager 
        private Fragment previousFragment = null;
        private Fragment fragment = null;

        private TextView toolbarTitle;
        private Android.Support.V7.Widget.Toolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_menu);
            GetReferencesFromLayout();
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            // loads initial fragment
            LoadFragment(FragmentId.Main_menu);
        }

        private void GetReferencesFromLayout()
        {
            toolbarTitle = FindViewById<TextView>(Resource.Id.toolbarTitle);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        }

        public override void OnBackPressed()
        {
            // leave default fragment in place
            if (FragmentManager.BackStackEntryCount > 1)
            {
                fragment = previousFragment;
                FragmentManager.PopBackStack();
            }
            else if (fragment is MainMenuFragment) 
            {
                FragmentManager childFm = fragment.ChildFragmentManager;
                // leave default fragment in place
                if (childFm.BackStackEntryCount > 1)
                {
                    childFm.PopBackStack();
                    return;
                }
            }
            else
            {
                Finish();   
            }
        }

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

        public void UpdateTitle(string title)
        {
            toolbarTitle.Text = title;
        }
    }
}