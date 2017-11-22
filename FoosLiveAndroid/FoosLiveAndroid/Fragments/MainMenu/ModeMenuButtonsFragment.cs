
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FoosLiveAndroid.Fragments
{
    public class ModeMenuButtonsFragment : Fragment
    {
        public new string Tag = "ModeMenuButtonsFragment";
        private const int VideoRequest = 0;

        private View view;
        private Button liveButton;
        private Button fromFileButton;

        private IOnFragmentInteractionListener interactionListener;

        public static Fragment NewInstance()
        {
            return new ModeMenuButtonsFragment();
        }

        public override void OnAttach(Context context)
        {
            try
            {
                interactionListener = (IOnFragmentInteractionListener)context;
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
            interactionListener.UpdateTitle(GetString(Resource.String.choose_mode));

            view = inflater.Inflate(Resource.Layout.mode_menu_items, container, false);

            GetReferencesFromLayout();

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
            liveButton = view.FindViewById<Button>(Resource.Id.liveButton);
            fromFileButton = view.FindViewById<Button>(Resource.Id.fromFileButton);
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == VideoRequest)
            {
                Intent intent = new Intent(Activity, typeof(GameActivity));
                // set video uri as game activity intent data
                intent.SetData(data.Data);
                StartActivity(intent);
            }
        }
    }
}
