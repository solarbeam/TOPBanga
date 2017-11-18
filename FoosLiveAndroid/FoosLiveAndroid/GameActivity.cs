
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FoosLiveAndroid
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity
    {
        private Button _gameButton;
        private TextView _score;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_game);
            //hides notification bar
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            GetReferencesFromLayout();
        }

        private void GetReferencesFromLayout()
        {
            _gameButton = FindViewById<Button>(Resource.Id.gameButton);
            _score = FindViewById<TextView>(Resource.Id.score);
        }
    }
}
