using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments.Interface
{
    /// <summary>
    /// This interface is used for communication between fragments and activitie
    /// </summary>
    public interface IOnFragmentInteractionListener
    {
        void LoadFragment(FragmentId id);
        void UpdateTitle(string title);
    }
}
