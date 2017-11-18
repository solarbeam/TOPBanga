using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FoosLiveAndroid.Fragments
{
    public class ModeFragment : Fragment
    {
        private View view;
        private Button liveButton;
        private Button fromFileButton;
        private ISwitchFragmentListener switchFragmentListener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override void OnAttach(Context context)
        {
            try
            {
                switchFragmentListener = (ISwitchFragmentListener)context;
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
            view = inflater.Inflate(Resource.Layout.fragment_buttons_mode, container, false);

            GetReferencesFromLayout();

            //liveButton.Click += delegate
            //{
            //    // load game activity

            //};

            //fromFileButton.Click += delegate
            //{
            //    // load game activity
            //};

            return view;
        }

        private void GetReferencesFromLayout()
        {
            liveButton = view.FindViewById<Button>(Resource.Id.liveButton);
            fromFileButton = view.FindViewById<Button>(Resource.Id.fromFileButton);
        }
    }
}
