using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments
{
    public class ModeMenuButtonsFragment : Fragment
    {
        // used for debugging
        public static new string Tag = "ModeMenuButtonsFragment";

        private View view;
        private Button liveButton;
        private Button fromFileButton;
        // target permissions list
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

            //set up click events
            liveButton.Click += InitialiseCameraActivity;
            fromFileButton.Click += StartVideoPickActivity;

            return view;
        }

        /// <summary>
        /// Starts the camera activity
        /// </summary>
        /// <param name="data">Video uri</param>
        private void StartCameraActivity(Android.Net.Uri data = null) 
        {
            Intent intent = new Intent(Activity, typeof(GameActivity));
            // set video uri as game activity intent data
            if (data != null)
                intent.SetData(data);
            StartActivity(intent);
        }


        private void InitialiseCameraActivity(object sender, EventArgs e) 
        {
            // Check android version
            if ((int)Build.VERSION.SdkInt < 23)
            {
                StartCameraActivity();
            }
            else
            {
                // API 23+ demands runtime requests
                GetCameraPermission();
            }
        }

        private void GetCameraPermission()
        {
            const string CameraPermission = Manifest.Permission.Camera;
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
                alert.SetTitle(GetString(Resource.String.camera_request_explanation_title));
                alert.SetMessage(GetString(Resource.String.camera_request_explanation_content));
                alert.SetButton(GetString(Resource.String.dismiss), (c, ev) =>
                {
                    alert.Dismiss();
                    RequestPermissions(PermissionsCamera, (int)ERequestId.Camera);
                });
                alert.Show();
                return;
            }
            //Finally request permissions with the list of permissions and Id
            RequestPermissions(PermissionsCamera, (int)ERequestId.Camera);
        }

        private void StartVideoPickActivity(object sender, EventArgs e)
        {
            var videoIntent = new Intent();
            videoIntent.SetAction(Intent.ActionPick);
            videoIntent.SetData(MediaStore.Video.Media.ExternalContentUri);
            StartActivityForResult(videoIntent, (int) ERequestId.VideoRequest);
        }

        private void GetReferencesFromLayout()
        {
            liveButton = view.FindViewById<Button>(Resource.Id.liveButton);
            fromFileButton = view.FindViewById<Button>(Resource.Id.fromFileButton);
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == (int)ERequestId.VideoRequest)
            {
                Snackbar.Make(view, "Not implemented", Snackbar.LengthLong)
                        .Show();
                //StartCameraActivity(data.Data);
            }
        }
        /// <summary>
        /// Called when permission request result is received
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="permissions">Permissions.</param>
        /// <param name="grantResults">Grant results.</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case (int)ERequestId.VideoRequest:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            StartCameraActivity();
                        }
                        else
                        {
                            // show notification about missing access
                            Snackbar.Make(view, 
                                          GetString(Resource.String.camera_access_missing),
                                          Snackbar.LengthLong)
                                    .Show();
                        }
                    }
                    break;
            }
        }

    }
}
