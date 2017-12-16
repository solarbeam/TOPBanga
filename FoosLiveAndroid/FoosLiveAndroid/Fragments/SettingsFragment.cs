using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using FoosLiveAndroid.Fragments.Interface;

namespace FoosLiveAndroid.Fragments
{
    enum SoundAsset
    {
        WinMario = 0,
        GoalMario = 1
    }
    public class SettingsFragment : Fragment
    {
        static readonly new string Tag = typeof(SettingsFragment).Name;

        private String GoalSoundMarioPath;
        private String WinSoundMarioPath;

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

        private void UpdateSelection()
        {
            // Assign preexisting values
            ISharedPreferences preferences = Context.GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);
            _team1ScoreSoundValue.Text = preferences.GetString("team1Score", "");
            _team1WinSoundValue.Text = preferences.GetString("team1Win", "");
            _team2ScoreSoundValue.Text = preferences.GetString("team2Score", "");
            _team2WinSoundValue.Text = preferences.GetString("team2Win", "");
            _soundSwitch.Checked = preferences.GetBoolean("soundEnabled", true);
            _syncSwitch.Checked = preferences.GetBoolean("syncEnabled", true);
            _team1Title.Text = preferences.GetString("team1Name", "TEAM1");
            _team2Title.Text = preferences.GetString("team2Name", "TEAM1"); 
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

            _team1ScoreSoundValue = _view.FindViewById<TextView>(Resource.Id.team1ScoreSoundValue);
            _team1WinSoundValue = _view.FindViewById<TextView>(Resource.Id.team1WinSoundValue);
            _team2ScoreSoundValue = _view.FindViewById<TextView>(Resource.Id.team2ScoreSoundValue);
            _team2WinSoundValue = _view.FindViewById<TextView>(Resource.Id.team2WinSoundValue);

            _team1Title = _view.FindViewById<TextView>(Resource.Id.team1Name);
            _team2Title = _view.FindViewById<TextView>(Resource.Id.team2Name);
            _team1TitleSettings = _view.FindViewById<RelativeLayout>(Resource.Id.team1TitleSettings);
            _team2TitleSettings = _view.FindViewById<RelativeLayout>(Resource.Id.team2TitleSettings);

            GoalSoundMarioPath = Context.GetString(Resource.String.defaultMarioGoalSound);
            WinSoundMarioPath = Context.GetString(Resource.String.defaultMarioWinSound);
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
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            _dialogBuilder = _dialogBuilder ?? new AlertDialog.Builder(Context);
            _dialogBuilder.SetTitle($"Choose {title}");

            _dialogBuilder.SetAdapter(adapter, (dialog, item) =>
            {
                ISharedPreferences preferences = Context.GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);
                ISharedPreferencesEditor prefsEditor = preferences.Edit();
                switch(item.Which)
                {
                    case (int)SoundAsset.GoalMario:
                        {
                            prefsEditor.PutString(title, WinSoundMarioPath).Apply();
                            break;
                        }
                    case (int)SoundAsset.WinMario:
                        {
                            prefsEditor.PutString(title, GoalSoundMarioPath).Apply();
                            break;
                        }
                    default:
                            break;
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
            InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
            imm.ToggleSoftInput(ShowFlags.Forced, 0);
        }

        private void HideKeyboard(EditText userInput)
        {
            InputMethodManager imm = (InputMethodManager)this.Activity.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(userInput.WindowToken, 0);
        }

        private void InputClicked(TextView title)
        {
            var inputDialog = new AlertDialog.Builder(Activity);
            EditText userInput = new EditText(Activity);

            string selectedInput = string.Empty;
            userInput.Text = title.Text;
            //SetEditTextStylings(userInput);
            userInput.InputType = Android.Text.InputTypes.TextVariationPersonName;
            inputDialog.SetTitle(selectedInput);
            inputDialog.SetView(userInput);
            inputDialog.SetPositiveButton(
                "Ok",
                (see, ess) =>
                {
                    if (userInput.Text != string.Empty)
                    {
                        title.Text = userInput.Text;
                        SaveUserInputToClass(userInput.Text, title);
                    }
                    HideKeyboard(userInput);
                });
            inputDialog.SetNegativeButton("Cancel", (afk, kfa) => { HideKeyboard(userInput); });
            inputDialog.Show();
            ShowKeyboard(userInput);
        }

        private void SaveUserInputToClass(string teamName, TextView teamNameTextView) 
        {
            if (teamNameTextView.Id == Resource.Id.team1Name)
            {
                _team1Title.Text = teamName;
                // Todo: make code modular
                ISharedPreferences preferences = Context.GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);
                ISharedPreferencesEditor prefsEditor = preferences.Edit();
                prefsEditor.PutString("team1Name", teamName);
                prefsEditor.Commit();
                prefsEditor.Dispose();
                preferences.Dispose();

            }
            else if (teamNameTextView.Id == Resource.Id.team2Name)
            {
                _team2Title.Text = teamName;
                // Todo: make code modular
                ISharedPreferences preferences = Context.GetSharedPreferences("FoosliveAndroid.dat", FileCreationMode.Private);
                ISharedPreferencesEditor prefsEditor = preferences.Edit();
                prefsEditor.PutString("team2Name", teamName);
                prefsEditor.Commit();
                prefsEditor.Dispose();
                preferences.Dispose();
            }
        }

    }
}
