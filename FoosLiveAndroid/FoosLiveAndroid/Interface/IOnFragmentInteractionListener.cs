using FoosLiveAndroid.Model;

namespace FoosLiveAndroid.Fragments
{
    public interface IOnFragmentInteractionListener
    {
        void LoadFragment(FragmentId id);
        void UpdateTitle(string title);
    }
}
