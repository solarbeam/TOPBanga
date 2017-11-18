using System;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments
{
    public class MainFragment : Fragment
    {
        private View view;
        private Button startButton;
        private Button settingsButton;
        private Button infoButton;
        private LinearLayout buttonContent;
        private LinearLayout modeContent;
        private ISwitchFragmentListener switchFragmentListener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override void OnAttach(Context context)
        {
            try
            {
                switchFragmentListener = (ISwitchFragmentListener) context;
            }
            catch (InvalidCastException e) 
            {
                Log.Error(Tag, "ISwitchFragmentListener not implemented in parent activity");
                throw e;
            }

            base.OnAttach(context);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_main_menu, container, false);

            GetReferencesFromLayout();

            startButton.Click += delegate 
            {
                buttonContent.Visibility = ViewStates.Gone;
                modeContent.Visibility = ViewStates.Visible;
                //switchFragmentListener.SwitchFragment(FragmentId.Mode);
            };

            settingsButton.Click += delegate 
            {
                switchFragmentListener.SwitchFragment(FragmentId.Settings);
            };

            infoButton.Click += delegate 
            {
                switchFragmentListener.SwitchFragment(FragmentId.Info);
            };

            return view;
        }

        private void GetReferencesFromLayout() 
        {
            startButton = view.FindViewById<Button>(Resource.Id.startButton);
            settingsButton = view.FindViewById<Button>(Resource.Id.settingsButton);
            infoButton = view.FindViewById<Button>(Resource.Id.infoButton);
            buttonContent = view.FindViewById<LinearLayout>(Resource.Id.buttonContent);
            modeContent = view.FindViewById<LinearLayout>(Resource.Id.menu_mode_buttons_temp);

        }
    }
}
