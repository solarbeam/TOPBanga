
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FoosLiveAndroid.Fragments
{
    public class ModeMenuButtonsFragment : Fragment
    {
        public static new string Tag = "ModeMenuButtonsFragment";
        private const int VideoRequest = 0;

        private View view;
        private Button liveButton;
        private Button fromFileButton;

        const int RequestCameraId = 0;
        readonly string [] PermissionsCamera = 
        {
          Manifest.Permission.Camera,
        };

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
                
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartCameraActivity();
                }
                else 
                {
                    GetCameraPermission();
                }

            };

            fromFileButton.Click += delegate
            {
                // select video file dialog
                StartVideoPickActivity();
            };

            return view;
        }

        private void GetCameraPermission()
        {
            const string CameraPermission = Manifest.Permission.Camera;
            //if (.CheckPermission(Manifest.Permission.Camera))
            if (Context.CheckSelfPermission(CameraPermission) == (int)Permission.Granted)
            {
                StartCameraActivity();
                return;
            }

            //need to request permission
            if (ShouldShowRequestPermissionRationale(CameraPermission))
            {
                //Explain to the user why we need to read the contacts
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(Context); 
                Android.App.AlertDialog alert = dialog.Create();  
                alert.SetTitle("Camera access");
                alert.SetMessage("Camera access is required to record and analyse live game.");

                alert.SetButton("OK", (c, ev) =>
                {
                    alert.Dismiss();
                    RequestPermissions(PermissionsCamera, RequestCameraId);
                });
                alert.Show();  
                return;
            }
            //Finally request permissions with the list of permissions and Id
            RequestPermissions(PermissionsCamera, RequestCameraId);
        }

        private void StartCameraActivity(Android.Net.Uri data) 
        {
            Intent intent = new Intent(Activity, typeof(GameActivity));
            // set video uri as game activity intent data
            intent.SetData(data);
            StartActivity(intent);
        }

        private void StartCameraActivity()
        {
            Intent intent = new Intent(Activity, typeof(GameActivity));
            StartActivity(intent);
        }

        private void StartVideoPickActivity()
        {
            var videoIntent = new Intent();
            videoIntent.SetAction(Intent.ActionPick);
            videoIntent.SetData(MediaStore.Video.Media.ExternalContentUri);
            StartActivityForResult(videoIntent, VideoRequest);
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
                StartCameraActivity(data.Data);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraId:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            StartCameraActivity();
                        }
                        else
                        {
                            Snackbar.Make(view, "The application does not have access to record live game", Snackbar.LengthLong)
                                    .Show();
                        }
                    }
                    break;
            }
        }
    }
}
