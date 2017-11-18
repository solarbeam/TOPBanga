using Android;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Fragments;

namespace FoosLiveAndroid
{
    //Todo: fragment backstack management
    //[Activity(Label = "Fooslive", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MenuActivity : AppCompatActivity, ISwitchFragmentListener
    {
        public const string Tag = "MenuActivity";
        private TextView toolbarTitle;
        private Android.Support.V7.Widget.Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_menu);
            GetReferencesFromLayout();
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            LoadInitialFragment();
        }

        private void GetReferencesFromLayout()
        {
            toolbarTitle = FindViewById<TextView>(Resource.Id.toolbarTitle);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        }

        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount > 0)
            {
                FragmentManager.PopBackStack();
            }
            else 
            {
                Finish();   
            }
        }

        private void LoadInitialFragment() {
            SwitchFragment(FragmentId.Main_menu);
        }

        public void SwitchFragment(int id)
        {
            Fragment fragment = null;
            //default target view to inflate
            int contentId = Resource.Id.menu_content;
            switch (id)
            {
                case FragmentId.Main_menu:
                    fragment = new MainFragment();
                    toolbarTitle.Text = GetString(Resource.String.main_menu);
                    break;
                case FragmentId.Mode:
                    //targets only button view
                    contentId = Resource.Id.buttonContent;
                    fragment = new ModeFragment();
                    toolbarTitle.Text = GetString(Resource.String.choose_mode);
                    break;
                case FragmentId.Settings:
                    //fragment = new SettingsFragment();
                    toolbarTitle.Text = GetString(Resource.String.settings);
                    break;
                case FragmentId.Info:
                    //fragment = new InfoFragment();
                    toolbarTitle.Text = GetString(Resource.String.info);
                    break;
                default:
                    Log.Error(Tag, $"SwitchFragment unknown ID: {id}");
                    // throw custom exception
                    break;
            }

            if (fragment == null)
            {
                return;
            }

            FragmentManager.BeginTransaction().Replace(contentId, fragment)
                           .Commit();
        }
    }
}