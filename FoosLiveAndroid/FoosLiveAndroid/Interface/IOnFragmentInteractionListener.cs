using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments
{
    /// <summary>
    /// This interface is used for communication between fragments and activitie
    /// </summary>
    public interface IOnFragmentInteractionListener
    {
        void UpdateTitle(string title);
        void LoadFragment(FragmentId id, bool saveState = true);
    }
}
