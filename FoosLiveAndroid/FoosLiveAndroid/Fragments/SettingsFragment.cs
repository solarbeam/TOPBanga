using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Android.Content.Res;
using FoosLiveAndroid.Fragments.Interface;

namespace FoosLiveAndroid.Fragments
{
    public class SettingsFragment : Fragment
    {
        static readonly new string Tag = typeof(SettingsFragment).Name;

        private View _view;
        private Switch _syncSwitch;
        private Switch _soundSwitch;
        private RelativeLayout _team1ScoreSoundItem;
        private TextView _team1ScoreSoundValue;
        private RelativeLayout _team1WinSoundItem;
        private TextView _team1WinSoundValue;
        private RelativeLayout _team2ScoreSoundItem;
        private TextView _team2ScoreSoundValue;
        private RelativeLayout _team2WinSoundItem;
        private TextView _team2WinSoundValue;

        private AlertDialog.Builder _dialogBuilder;

        private IOnFragmentInteractionListener _interactionListener;

        public static Fragment NewInstance()
        {
            return new SettingsFragment();
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
            _interactionListener.UpdateTitle(GetString(Resource.String.settings));
            _view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);

            GetReferencesFromLayout();

            // Todo: set up button click events

            //Todo: set up sound adapter from model
            var scoreSoundsAdapter = new ArrayAdapter<string>(
                Context, Android.Resource.Layout.SimpleListItem1, new string[] { "sound1", "sound2" });

            var winSoundsAdapter = new ArrayAdapter<string>(
                Context, Android.Resource.Layout.SimpleListItem1, new string[] { "sound1", "sound2" });

            // Todo: set up button click events
            _team1ScoreSoundItem.Click += delegate
            {
                OpenSoundPicker("sound", scoreSoundsAdapter);
            };

            _team1WinSoundItem.Click += delegate
            {
                OpenSoundPicker("sound", winSoundsAdapter);
            };

            _team2ScoreSoundItem.Click += delegate
            {
                OpenSoundPicker("sound", scoreSoundsAdapter);
            };

            _team2WinSoundItem.Click += delegate
            {
                OpenSoundPicker("sound", winSoundsAdapter);
            };

            //Todo: bind switches with evens
            _syncSwitch.CheckedChange += delegate {
                Toast.MakeText(Context, "sync event", ToastLength.Short).Show();
            };

            _soundSwitch.CheckedChange += delegate {
                Toast.MakeText(Context, "sound event", ToastLength.Short).Show();
            };

            return _view;
        }

        private void GetReferencesFromLayout()
        {
            _syncSwitch = _view.FindViewById<Switch>(Resource.Id.syncSwitch);
            _soundSwitch = _view.FindViewById<Switch>(Resource.Id.soundSwitch);

            _team1ScoreSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team1ScoreSoundItem);
            _team1WinSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team1WinSoundItem);
            _team2ScoreSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team2ScoreSoundItem);
            _team2WinSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team2WinSoundItem);

            _team1ScoreSoundValue = _view.FindViewById<TextView>(Resource.Id.team1ScoreSoundValue);
            _team1WinSoundValue = _view.FindViewById<TextView>(Resource.Id.team1WinSoundValue);
            _team2ScoreSoundValue = _view.FindViewById<TextView>(Resource.Id.team2ScoreSoundValue);
            _team2WinSoundValue = _view.FindViewById<TextView>(Resource.Id.team2WinSoundValue);
        }

        //Todo set values from model/cfg/shared pref
        private void RestoreCurrentSoundValues()
        {
            _team1ScoreSoundValue.Text = "Demo sound";
            _team1WinSoundValue.Text = "Demo sound";
            _team2ScoreSoundValue.Text = "Demo sound";
            _team2WinSoundValue.Text = "Demo sound";
        }

        // Todo: fully implement Alertdialog and selection events
        private void OpenSoundPicker(string title, ArrayAdapter<string> adapter)
        {
            if (adapter == null) throw new ArgumentNullException(nameof(adapter));
            _dialogBuilder = _dialogBuilder ?? new AlertDialog.Builder(Context);
            _dialogBuilder.SetTitle($"Choose {title}");
            _dialogBuilder.SetAdapter(adapter, (dialog, item) =>
            {
                
            });
            var soundPickDialog = _dialogBuilder.Create();
            soundPickDialog.Show();
        }
    }
}
