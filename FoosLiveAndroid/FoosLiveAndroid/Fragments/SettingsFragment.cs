using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FoosLiveAndroid.Fragments.Interface;
using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments
{
    public class SettingsFragment : Fragment
    {
        static readonly new string Tag = typeof(SettingsFragment).Name;

        private String GoalSoundMarioPath;
        private String WinSoundMarioPath;

        private View _view;
        private Switch _syncSwitch;
        private Switch _soundSwitch;
        private RelativeLayout _team1ScoreSoundItem;
        private TextView _team1GoalSoundValue;
        private RelativeLayout _team1WinSoundItem;
        private TextView _team1WinSoundValue;
        private RelativeLayout _team2ScoreSoundItem;
        private TextView _team2GoalSoundValue;
        private RelativeLayout _team2WinSoundItem;
        private TextView _team2WinSoundValue;

        private RelativeLayout _team1TitleSettings;
        private RelativeLayout _team2TitleSettings;
        private TextView _team1Title;
        private TextView _team2Title;

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
                Context, Android.Resource.Layout.SimpleListItem1, new string[] { "Mario Win Sound", "Mario Goal Sound" });

            var winSoundsAdapter = new ArrayAdapter<string>(
                Context, Android.Resource.Layout.SimpleListItem1, new string[] { "Mario Win Sound", "Mario Goal Sound" });

            // Todo: set up button click events
            _team1ScoreSoundItem.Click += delegate
            {
                OpenSoundPicker("team1Score", scoreSoundsAdapter);
            };

            _team1WinSoundItem.Click += delegate
            {
                OpenSoundPicker("team1Win", winSoundsAdapter);
            };

            _team2ScoreSoundItem.Click += delegate
            {
                OpenSoundPicker("team2Score", scoreSoundsAdapter);
            };

            _team2WinSoundItem.Click += delegate
            {
                OpenSoundPicker("team2Win", winSoundsAdapter);
            };

            UpdateSelection();

            //Todo: bind switches with events
            _syncSwitch.CheckedChange += delegate {
                ISharedPreferences preferences = Context.GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);
                ISharedPreferencesEditor editor = preferences.Edit();
                editor.PutBoolean("syncEnabled", _syncSwitch.Checked).Apply();
                editor.Commit();
                editor.Dispose();
                preferences.Dispose();
            };
            _soundSwitch.CheckedChange += delegate {
                ISharedPreferences preferences = Context.GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);
                ISharedPreferencesEditor editor = preferences.Edit();
                editor.PutBoolean("soundEnabled", _soundSwitch.Checked).Apply();
                editor.Commit();
                editor.Dispose();
                preferences.Dispose();
            };

            _team1TitleSettings.Click += delegate
            {
                InputClicked(_team1Title);
            };
            _team2TitleSettings.Click += delegate
            {
                InputClicked(_team2Title);
            };

            return _view;
        }

        /// Assign preexisting values
        private void UpdateSelection()
        {
            //Extract default values from resources
            var team1GoalSoundDefault = GetString(Resource.String.saved_team1_goal_default);
            var team2GoalSoundDefault = GetString(Resource.String.saved_team2_goal_default);
            var team1WinSoundDefault = GetString(Resource.String.saved_team1_win_default);
            var team2WinSoundDefault = GetString(Resource.String.saved_team2_win_default);
            var soundSwitchDefault = Resources.GetBoolean(Resource.Boolean.saved_sound_enabled_default);
            var syncSwitchDefault = Resources.GetBoolean(Resource.Boolean.saved_sync_enabled_default);
            var team1NameDefault = GetString(Resource.String.saved_team1_name_default);
            var team2NameDefault = GetString(Resource.String.saved_team2_name_default);

            // Extract and assign values from sharedPreferences
            var preferences = Context.GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);

            _team1GoalSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team1_goal), team1GoalSoundDefault);
            _team1WinSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team1_win), team1WinSoundDefault);
            _team2GoalSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team2_goal), team2GoalSoundDefault);
            _team2WinSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team2_win), team2WinSoundDefault);
            _soundSwitch.Checked = preferences.GetBoolean(GetString(Resource.String.saved_sound_enabled), soundSwitchDefault);
            _syncSwitch.Checked = preferences.GetBoolean(GetString(Resource.String.saved_sync_enabled), syncSwitchDefault);
            _team1Title.Text = preferences.GetString(GetString(Resource.String.saved_team1_name), team1NameDefault);
            _team2Title.Text = preferences.GetString(GetString(Resource.String.saved_team2_name), team2NameDefault); 
            preferences.Dispose();
        }

        private void GetReferencesFromLayout()
        {
            _syncSwitch = _view.FindViewById<Switch>(Resource.Id.syncSwitch);
            _soundSwitch = _view.FindViewById<Switch>(Resource.Id.soundSwitch);

            _team1ScoreSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team1ScoreSoundItem);
            _team1WinSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team1WinSoundItem);
            _team2ScoreSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team2ScoreSoundItem);
            _team2WinSoundItem = _view.FindViewById<RelativeLayout>(Resource.Id.team2WinSoundItem);

            _team1GoalSoundValue = _view.FindViewById<TextView>(Resource.Id.team1ScoreSoundValue);
            _team1WinSoundValue = _view.FindViewById<TextView>(Resource.Id.team1WinSoundValue);
            _team2GoalSoundValue = _view.FindViewById<TextView>(Resource.Id.team2ScoreSoundValue);
            _team2WinSoundValue = _view.FindViewById<TextView>(Resource.Id.team2WinSoundValue);

            _team1Title = _view.FindViewById<TextView>(Resource.Id.team1Name);
            _team2Title = _view.FindViewById<TextView>(Resource.Id.team2Name);
            _team1TitleSettings = _view.FindViewById<RelativeLayout>(Resource.Id.team1TitleSettings);
            _team2TitleSettings = _view.FindViewById<RelativeLayout>(Resource.Id.team2TitleSettings);
        }

        private void OpenSoundPicker(string soundItem, ArrayAdapter<string> adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            _dialogBuilder = _dialogBuilder ?? new AlertDialog.Builder(Context);
            _dialogBuilder.SetTitle($"Choose {soundItem}");

            _dialogBuilder.SetAdapter(adapter, (dialog, item) =>
            {
                var preferences = Context.GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);
                var prefsEditor = preferences.Edit();
                switch(item.Which)
                {
                    case (int)SoundAsset.GoalMario:
                        {
                            prefsEditor.PutString(soundItem, GetString(Resource.String.mario_goal_sound)).Apply();
                            break;
                        }
                    case (int)SoundAsset.WinMario:
                        {
                            prefsEditor.PutString(soundItem, GetString(Resource.String.mario_win_sound)).Apply();
                            break;
                        }
                }
                prefsEditor.Commit();
                prefsEditor.Dispose();
                preferences.Dispose();
                UpdateSelection();
            });

            var soundPickDialog = _dialogBuilder.Create();
            soundPickDialog.Show();
        }

        private void ShowKeyboard(EditText userInput)
        {
            userInput.RequestFocus();
            var inputMethodManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, 0);
        }

        private void HideKeyboard(EditText userInput)
        {
            var inputMethodManager = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(userInput.WindowToken, 0);
        }

        private void InputClicked(TextView teamTitle)
        {
            var inputDialog = new AlertDialog.Builder(Activity);
            EditText userInput = new EditText(Activity);

            string selectedInput = string.Empty;
            userInput.Text = teamTitle.Text;
            //SetEditTextStylings(userInput);
            userInput.InputType = Android.Text.InputTypes.TextVariationPersonName;
            inputDialog.SetTitle(selectedInput);
            inputDialog.SetView(userInput);
            inputDialog.SetPositiveButton(
                GetString(Resource.String.ok),
                (see, ess) =>
                {
                    if (userInput.Text != string.Empty)
                    {
                        teamTitle.Text = userInput.Text;
                        SaveUserInput(userInput.Text, teamTitle.Id);
                    }
                    HideKeyboard(userInput);
                });
            inputDialog.SetNegativeButton(GetString(Resource.String.cancel), 
                                          (obj, args) => { HideKeyboard(userInput); });
            inputDialog.Show();
            ShowKeyboard(userInput);
        }

        private void SaveUserInput(string teamName, int teamTitleId) 
        {
            string key;
            if (teamTitleId == _team1Title.Id)
                key = GetString(Resource.String.saved_team1_name);
            else
                key = GetString(Resource.String.saved_team2_name);
            
            var preferences = Context.GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);
            var prefsEditor = preferences.Edit();
            prefsEditor.PutString(key, teamName);
            prefsEditor.Commit();
            prefsEditor.Dispose();
            preferences.Dispose();
        }
    }
}
