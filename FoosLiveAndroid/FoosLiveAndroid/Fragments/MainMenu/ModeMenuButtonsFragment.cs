﻿using System;
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
using FoosLiveAndroid.Fragments.Interface;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments.MainMenu
{
    public class ModeMenuButtonsFragment : Fragment
    {
        static readonly new string Tag = typeof(ModeMenuButtonsFragment).Name;

        private View _view;
        private Button _liveButton;
        private Button _fromFileButton;
        // target permissions list
        private readonly string [] _permissionsCamera = 
        {
          Manifest.Permission.Camera
        };

        private IOnFragmentInteractionListener _interactionListener;

        public static Fragment NewInstance()
        {
            return new ModeMenuButtonsFragment();
        }

        public override void OnAttach(Context context)
        {
            try
            {
                _interactionListener = (IOnFragmentInteractionListener)context;
            }
            catch (InvalidCastException)
            {
                Log.Error(Tag, "IOnFragmentInteractionListener not implemented in parent activity");
                throw;
            }

            base.OnAttach(context);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _interactionListener.UpdateTitle(GetString(Resource.String.choose_mode));

            _view = inflater.Inflate(Resource.Layout.mode_menu_items, container, false);

            GetReferencesFromLayout();

            //Set up click events
            _liveButton.Click += InitialiseCameraActivity;
            _fromFileButton.Click += StartVideoPickActivity;

            return _view;
        }

        /// <summary>
        /// Starts the camera activity
        /// </summary>
        /// <param name="data">Video uri</param>
        private void StartCameraActivity(Android.Net.Uri data = null) 
        {
            var intent = new Intent(Activity, typeof(GameActivity));
            // Set video uri as game activity intent data
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
            const string cameraPermission = Manifest.Permission.Camera;
            if (Context.CheckSelfPermission(cameraPermission) == (int)Permission.Granted)
            {
                StartCameraActivity();
                return;
            }

            //need to request permission
            if (ShouldShowRequestPermissionRationale(cameraPermission))
            {
                //Explain to the user why we need to use the camera
                var dialog = new AlertDialog.Builder(Context);
                var alert = dialog.Create();
                alert.SetTitle(GetString(Resource.String.camera_request_explanation_title));
                alert.SetMessage(GetString(Resource.String.camera_request_explanation_content));
                alert.SetButton(GetString(Resource.String.ok), (c, ev) =>
                {
                    alert.Dismiss();
                    RequestPermissions(_permissionsCamera, (int)RequestId.Camera);
                });
                alert.Show();
                return;
            }
            //Finally request permissions with the list of permissions and Id
            RequestPermissions(_permissionsCamera, (int)RequestId.Camera);
        }

        private void StartVideoPickActivity(object sender, EventArgs e)
        {
            var videoIntent = new Intent();
            videoIntent.SetAction(Intent.ActionPick);
            videoIntent.SetData(MediaStore.Video.Media.ExternalContentUri);
            StartActivityForResult(videoIntent, (int) RequestId.VideoRequest);
        }

        private void GetReferencesFromLayout()
        {
            _liveButton = _view.FindViewById<Button>(Resource.Id.liveButton);
            _fromFileButton = _view.FindViewById<Button>(Resource.Id.fromFileButton);
        }

        /// <summary>
        /// Called on result of VideoPickActivity 
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                switch(requestCode) 
                {
                    case (int)RequestId.VideoRequest:
                        StartCameraActivity(data.Data);
                        break;
                    default:
                        Log.Wtf(Tag, "Unknown activity result request: ", requestCode);
                        break;
                }
            }
        }
        /// <summary>
        /// Called when permission request result is received
        /// </summary>
        /// <param name="requestCode">The request code passed in RequestPermissions(permission, code)</param>
        /// <param name="permissions">The requested permissions</param>
        /// <param name="grantResults">The grant results for the corresponding permissions which is either Permission.Granted or Permission.Denied</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == (int)RequestId.Camera)
            {
                if (grantResults[0] == Permission.Granted)
                    StartCameraActivity();
                else 
                    // show notification about missing access
                    Snackbar.Make(_view,
                                  GetString(Resource.String.camera_access_missing),
                                  Snackbar.LengthLong)
                            .Show();
            }
        }

    }
}
