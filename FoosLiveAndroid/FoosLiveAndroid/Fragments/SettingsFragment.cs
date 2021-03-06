﻿using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FoosLiveAndroid.Fragments.Interface;
using FoosLiveAndroid.Model;
using FoosLiveAndroid.Util.Sounds;

namespace FoosLiveAndroid.Fragments
{
    public class SettingsFragment : Fragment
    {
        static readonly new string Tag = typeof(SettingsFragment).Name;

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
        private Button _logoutButton;

        private RelativeLayout _team1TitleSettings;
        private RelativeLayout _team2TitleSettings;
        private TextView _team1Title;
        private TextView _team2Title;

        private PlayerOgg previewPlayer;

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
      
            // Todo: use string resources instead
            var soundsAdapter = new ArrayAdapter<string>(
                Context, Android.Resource.Layout.SimpleListItem1, 
                new[] { SoundAsset.GoalMario.ToString(), SoundAsset.WinMario.ToString(),
                SoundAsset.BingSound.ToString(), SoundAsset.CrowdCheer.ToString() });

            _logoutButton.Click += delegate {
                var intent = new Intent(Application.Context, typeof(LoginActivity));
                intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
                StartActivity(intent);
            };

            _team1ScoreSoundItem.Click += delegate
            {
                OpenSoundPicker(GetString(Resource.String.team1_score_sound_label), 
                                soundsAdapter, _team1GoalSoundValue, 
                                GetString(Resource.String.saved_team1_goal));
            };

            _team1WinSoundItem.Click += delegate
            {
                OpenSoundPicker(GetString(Resource.String.team1_win_sound_label),
                                soundsAdapter, _team1WinSoundValue,
                                GetString(Resource.String.saved_team1_win));
            };

            _team2ScoreSoundItem.Click += delegate
            {
                OpenSoundPicker(GetString(Resource.String.team2_score_sound_label),
                                soundsAdapter, _team2GoalSoundValue,
                                GetString(Resource.String.saved_team1_goal));
            };

            _team2WinSoundItem.Click += delegate
            {
                OpenSoundPicker(GetString(Resource.String.team2_win_sound_label),
                                soundsAdapter, _team2WinSoundValue,
                                GetString(Resource.String.saved_team2_win));
            };

            UpdateSelection();

            _syncSwitch.CheckedChange += delegate
            {
                SaveSwitchValue(_syncSwitch);
            };
            _soundSwitch.CheckedChange += delegate {
                SaveSwitchValue(_soundSwitch);
            };

            _team1TitleSettings.Click += delegate
            {
                RequestTitle(_team1Title);
            };
            _team2TitleSettings.Click += delegate
            {
                RequestTitle(_team2Title);
            };

            return _view;
        }

        /// Assign preexisting values
        private void UpdateSelection()
        {
            //Extract default values from resources

            //Todo: use string resources instead of enum
            var soundSwitchDefault = Resources.GetBoolean(Resource.Boolean.saved_sound_enabled_default);
            var syncSwitchDefault = Resources.GetBoolean(Resource.Boolean.saved_sync_enabled_default);
            var team1NameDefault = GetString(Resource.String.saved_team1_name_default);
            var team2NameDefault = GetString(Resource.String.saved_team2_name_default);

            // Extract and assign values from sharedPreferences
            var preferences = Context.GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);

            _team1GoalSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team1_goal), SoundAsset.GoalMario.ToString());
            _team1WinSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team1_win), SoundAsset.WinMario.ToString());
            _team2GoalSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team2_goal), SoundAsset.GoalMario.ToString());
            _team2WinSoundValue.Text = preferences.GetString(GetString(Resource.String.saved_team2_win), SoundAsset.WinMario.ToString());
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
            _logoutButton = _view.FindViewById<Button>(Resource.Id.logoutButton);
        }

        private void OpenSoundPicker(string soundItem, ArrayAdapter<string> adapter, TextView soundTitle, string sharedPrefKey)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            _dialogBuilder = _dialogBuilder ?? new AlertDialog.Builder(Context);
            _dialogBuilder.SetTitle($"Choose {soundItem} sound");

            _dialogBuilder.SetAdapter(adapter, (dialog, item) =>
            {
                var preferences = Context.GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);
                var prefsEditor = preferences.Edit();
                switch (item.Which)
                {
                    case (int)SoundAsset.GoalMario:
                        {
                            previewPlayer = new PlayerOgg(FilePathResolver.GetFile(Context, SoundAsset.GoalMario.ToString()));
                            prefsEditor.PutString(sharedPrefKey, SoundAsset.GoalMario.ToString()).Apply();
                            soundTitle.Text = SoundAsset.GoalMario.ToString();
                            break;
                        }
                    case (int)SoundAsset.WinMario:
                        {
                            previewPlayer = new PlayerOgg(FilePathResolver.GetFile(Context, SoundAsset.WinMario.ToString()));
                            prefsEditor.PutString(sharedPrefKey, SoundAsset.WinMario.ToString()).Apply();
                            soundTitle.Text = SoundAsset.WinMario.ToString();
                            break;
                        }
                    case (int)SoundAsset.BingSound:
                        {
                            previewPlayer = new PlayerOgg(FilePathResolver.GetFile(Context, SoundAsset.BingSound.ToString()));
                            prefsEditor.PutString(sharedPrefKey, SoundAsset.BingSound.ToString());
                            soundTitle.Text = SoundAsset.BingSound.ToString();
                            break;
                        }
                    case (int)SoundAsset.CrowdCheer:
                        {
                            previewPlayer = new PlayerOgg(FilePathResolver.GetFile(Context, SoundAsset.CrowdCheer.ToString()));
                            prefsEditor.PutString(sharedPrefKey, SoundAsset.CrowdCheer.ToString());
                            soundTitle.Text = SoundAsset.CrowdCheer.ToString();
                            break;
                        }
                }
                if (!prefsEditor.Commit())
                    Log.Error(Tag, "Failed to save user sound selection.");

                // Preview the sound
                previewPlayer.Play();

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

        private void RequestTitle(TextView teamTitle)
        {
            var inputDialog = new AlertDialog.Builder(Activity);
            var userInput = new EditText(Activity);

            var selectedInput = string.Empty;
            userInput.Text = teamTitle.Text;
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

                        string key;
                        if (teamTitle.Id == _team1Title.Id)
                            key = GetString(Resource.String.saved_team1_name);
                        else
                            key = GetString(Resource.String.saved_team2_name);
                        SaveValue(key,userInput.Text);
                    }
                    HideKeyboard(userInput);
                });
            inputDialog.SetNegativeButton(GetString(Resource.String.cancel),
                                          (obj, args) => { HideKeyboard(userInput); });
            inputDialog.Show();
            ShowKeyboard(userInput);
        }

        private void SaveSwitchValue(Switch settingsSwitch)
        {
            string switchPrefKey;
            if (settingsSwitch.Id == Resource.Id.soundSwitch)
                switchPrefKey = GetString(Resource.String.saved_sound_enabled);
            else
                switchPrefKey = GetString(Resource.String.saved_sync_enabled);
            
            SaveValue(switchPrefKey, settingsSwitch.Checked);
        }

        private void SaveValue<T>(string prefKey, T input)
        {
            var preferences = Context.GetSharedPreferences(GetString(Resource.String.preference_file_key), FileCreationMode.Private);
            var editor = preferences.Edit();
            if (input is bool)
            {
                var parsedInput = Convert.ToBoolean(input);
                editor.PutBoolean(prefKey, parsedInput).Apply();
            }
            else if (input is string)
            {
                var parsedInput = Convert.ToString(input);
                editor.PutString(prefKey, parsedInput).Apply();
            }

            editor.Commit();
            editor.Dispose();
            preferences.Dispose();
        }
    }
}
