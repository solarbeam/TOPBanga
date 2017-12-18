using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Fragments;
using Android.Content.PM;
using FoosLiveAndroid.Fragments.MainMenu;
using FoosLiveAndroid.Fragments.Interface;
using FoosLiveAndroid.Util.Login;

namespace FoosLiveAndroid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
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
            var lm = LoginManager.GetInstance(this);
            SetContentView(Resource.Layout.activity_menu);
            GetReferencesFromLayout();
            SetSupportActionBar(_toolbar);
            // hide default top bar title
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            // load initial fragment
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.menu_content, _fragment = MainMenuFragment.NewInstance())
                           .Commit();


            var userData = Intent.GetBundleExtra(GetString(Resource.String.google_user_data_key));
            //Todo: hook data to the new DB
            var userId = userData.GetString(GetString(Resource.String.google_id_key));
            var userName = userData.GetString(GetString(Resource.String.google_id_name));

            Toast.MakeText(this, userId, ToastLength.Long).Show();
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
        /// <param name="id">Fragment identifier</param>
        /// <param name="saveState">If set to <c>true</c> save state for navigation</param>
        public void LoadFragment(FragmentId id, bool saveState = true)
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
                case FragmentId.History:
                    _fragment = HistoryFragment.NewInstance();
                    break;
                case FragmentId.Info:
                    _fragment = InfoFragment.NewInstance();
                    break;
                default:
                    Log.Wtf(Tag, $"SwitchFragment unknown ID: {id}");
                    break;
            }

            if (_fragment == null) return;
            var transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.menu_content, _fragment);
            if (saveState)
                transaction.AddToBackStack(_fragment.Tag);
            transaction.Commit();
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
