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
        public bool ChooseModeState;
        private const int VideoRequest = 0;
        private View view;

        private Button startButton;
        private Button settingsButton;
        private Button infoButton;
        private Button liveButton;
        private Button fromFileButton;

        private LinearLayout buttonsMain;
        private LinearLayout buttonsMode;

        private IOnFragmentInteractionListener interactionListener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

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
            view = inflater.Inflate(Resource.Layout.fragment_main_menu, container, false);
            GetReferencesFromLayout();
             //Main menu buttons
            startButton.Click += delegate 
            {
                interactionListener.UpdateTitle(GetString(Resource.String.choose_mode));
                LoadModeMenu();
            };

            settingsButton.Click += delegate 
            {
                interactionListener.LoadFragment(FragmentId.Settings);
            };

            infoButton.Click += delegate 
            {
                interactionListener.LoadFragment(FragmentId.Info);
            };


             //Mode menu buttons
            liveButton.Click += delegate
            {
                Intent intent = new Intent(Activity, typeof(GameActivity));
                StartActivity(intent);
            };

            fromFileButton.Click += delegate
            {
                // select video file dialog
                var videoIntent = new Intent();
                videoIntent.SetAction(Intent.ActionPick);
                videoIntent.SetData(MediaStore.Video.Media.ExternalContentUri);
                StartActivityForResult(videoIntent, VideoRequest);
            };

            return view;
        }

        private void GetReferencesFromLayout() 
        {
            buttonsMain = view.FindViewById<LinearLayout>(Resource.Id.buttonsMain);
            startButton = view.FindViewById<Button>(Resource.Id.startButton);
            settingsButton = view.FindViewById<Button>(Resource.Id.settingsButton);
            infoButton = view.FindViewById<Button>(Resource.Id.infoButton);

            buttonsMode = view.FindViewById<LinearLayout>(Resource.Id.buttonsMode);
            liveButton = view.FindViewById<Button>(Resource.Id.liveButton);
            fromFileButton = view.FindViewById<Button>(Resource.Id.fromFileButton);

        }

        public void LoadModeMenu() 
        {
            ChooseModeState = true;
            buttonsMain.Visibility = ViewStates.Gone;
            buttonsMode.Visibility = ViewStates.Visible;
        }

        public void LoadMainMenu() 
        {
            ChooseModeState = false;
            buttonsMain.Visibility = ViewStates.Visible;
            buttonsMode.Visibility = ViewStates.Gone;
        }


        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == VideoRequest)
            {
                Intent intent = new Intent(Activity, typeof(GameActivity));
                //Todo: check if the data transfer is valid
                intent.SetData(data.Data);                
                StartActivity(intent);
            }
        }

    }
}
