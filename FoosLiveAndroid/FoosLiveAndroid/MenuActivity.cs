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
    [Activity(Label = "Fooslive", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MenuActivity : AppCompatActivity, IOnFragmentInteractionListener
    {
        public const string Tag = "MenuActivity";
        Fragment fragment = null;
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
            // leave default fragment main menu[0] in place
            if (FragmentManager.BackStackEntryCount > 1)
            {
                FragmentManager.PopBackStack();
            }
            else if (fragment is MainMenuFragment) 
            {
                ((MainMenuFragment)fragment).LoadMainMenu();
            }
            else
            {
                Finish();   
            }
        }

        private void LoadInitialFragment() {
            LoadFragment(FragmentId.Main_menu);
        }

        public void LoadFragment(int id)
        {
            fragment = null;
            switch (id)
            {
                case FragmentId.Main_menu:
                    fragment = new MainMenuFragment();
                    toolbarTitle.Text = GetString(Resource.String.main_menu);

                    break;
                case FragmentId.Settings:
                    fragment = new SettingsFragment();
                    toolbarTitle.Text = GetString(Resource.String.settings);
                    break;
                case FragmentId.Info:
                    fragment = new InfoFragment();
                    toolbarTitle.Text = GetString(Resource.String.info);
                    break;
                default:
                    Log.Error(Tag, $"SwitchFragment unknown ID: {id}");
                    // throw custom exception
                    break;
            }

            if (fragment != null)
            {

                var transaction = FragmentManager.BeginTransaction();
                transaction.AddToBackStack(fragment.Tag);
                transaction.Replace(Resource.Id.menu_content, fragment).Commit();
            }
        }

        public void UpdateTitle(string title)
        {
            toolbarTitle.Text = title;
        }
    }
}