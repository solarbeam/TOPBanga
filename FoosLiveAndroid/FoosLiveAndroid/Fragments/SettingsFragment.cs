using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Android.Content.Res;

namespace FoosLiveAndroid.Fragments
{
    public class SettingsFragment : Fragment
    {
        static readonly new string Tag = typeof(SettingsFragment).Name;

        private View _view;
        private Button _team1ScoreSoundButton;
        private Button _team1WinSoundButton;
        private Button _team2ScoreSoundButton;
        private Button _team2WinSoundButton;

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
            _team1ScoreSoundButton.Click += delegate
            {
                OpenSoundPicker("title", scoreSoundsAdapter);
            };

            _team1WinSoundButton.Click += delegate
            {
                OpenSoundPicker("title", winSoundsAdapter);
            };

            return _view;
        }

        private void GetReferencesFromLayout()
        {
            _team1ScoreSoundButton = _view.FindViewById<Button>(Resource.Id.team1ScoreSoundButton);
            _team1WinSoundButton = _view.FindViewById<Button>(Resource.Id.team1WinSoundButton);
            _team2ScoreSoundButton = _view.FindViewById<Button>(Resource.Id.team2ScoreSoundButton);
            _team2WinSoundButton = _view.FindViewById<Button>(Resource.Id.team2WinSoundButton);
        }

        //Todo set values from model/cfg/shared pref
        private void RestoreCurrentSoundValues()
        {
            
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
