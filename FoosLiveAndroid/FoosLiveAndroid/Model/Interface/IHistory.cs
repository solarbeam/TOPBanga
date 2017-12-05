using System;
namespace FoosLiveAndroid.Model.Interface
{
    public interface IHistory
    {
        string BlueTeamName { get; set; }
        string RedTeamName { get; set; }
        int BlueTeamPoints { get; set; }
        int RedTeamPoints { get; set; }
    }
}
