using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace FoosLiveAndroid.Fragments
{
    public class SettingsFragment : Fragment
    {
        public static new string Tag = "SettingsFragment";

        private View view;
        private Button team1ScoreSoundButton;
        private Button team1WinSoundButton;
        private Button team2ScoreSoundButton;
        private Button team2WinSoundButton;

        AlertDialog.Builder dialogBuilder;

        private IOnFragmentInteractionListener interactionListener;

        public static Fragment NewInstance()
        {
            return new SettingsFragment();
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
            interactionListener.UpdateTitle(GetString(Resource.String.settings));
            view = inflater.Inflate(Resource.Layout.fragment_settings, container, false);

            GetReferencesFromLayout();

            return view;
        }

        private void GetReferencesFromLayout()
        {
            //team1ScoreSoundButton = view.FindViewById<Button>(Resource.Id.team1ScoreSoundSpinner);
            //team1WinSoundButton  = view.FindViewById<Button>(Resource.Id.team1WinSoundSpinner);
            //team2ScoreSoundButton = view.FindViewById<Button>(Resource.Id.team2ScoreSoundSpinner);
            //team2WinSoundButton  = view.FindViewById<Button>(Resource.Id.team2WinSoundSpinner);
        }

        private void SoundItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            // Todo: change game sounds based on request
        }

        private void OpenSoundPicker(ArrayAdapter<string> adapter)
        {
            if (adapter == null) throw new ArgumentNullException(nameof(adapter));
            dialogBuilder =  dialogBuilder ?? new AlertDialog.Builder(Context);
            dialogBuilder.SetTitle("");
            dialogBuilder.SetAdapter(adapter, (dialog, item) => {
                //button.Text(adapter.getItem(item));
            });
            AlertDialog soundPickDialog = dialogBuilder.Create();
            soundPickDialog.Show();
        }
    }
}
