using Android.Util;
using FoosLiveAndroid.Model.Interface;
using System;

namespace FoosLiveAndroid.Model
{
    public class History : IHistory
    {
        public string BlueTeamName { get; set; }
        public string RedTeamName { get; set; }
        public int BlueTeamPoints { get; set; }
        public int RedTeamPoints { get; set; }
        public DateTime DateTime { get; set; } 

        public History(string[] input)
        {
            BlueTeamName = input[0];
            RedTeamName = input[1];
            BlueTeamPoints = int.Parse(input[2]);
            RedTeamPoints = int.Parse(input[3]);
            DateTime = DateTime.ParseExact(input[4], "yyyy-MM-dd HH:mm:ss", null);
        }
    }
}
