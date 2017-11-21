namespace FoosLiveAndroid.Fragments
{
    public interface IOnFragmentInteractionListener
    {
        void LoadFragment(int id);
        void UpdateTitle(string title);
    }
}
