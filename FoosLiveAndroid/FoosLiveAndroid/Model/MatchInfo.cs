using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FoosLiveAndroid.Model
{
    [Serializable]
    class MatchInfo
    {
        public string teamOne { get; set; }
        public string teamTwo { get; set; }
        public int teamOneScore { get; set; }
        public int teamTwoScore { get; set; }
        public double maxSpeed { get; set; }
        public double avgSpeed { get; set; }
        public int[] zones { get; set; }
        public MatchInfo(string teamOneName, int teamOneScoreAchieved, string teamTwoName, int teamTwoScoreAchieved,
                            double maxSpeedAchieved, double avgSpeedAchieved, int[] zoneInfo)
        {
            // Assign team one info
            teamOne = teamOneName;
            teamOneScore = teamOneScoreAchieved;

            // Assign team two info
            teamTwo = teamTwoName;
            teamTwoScore = teamTwoScoreAchieved;

            // Assign match info
            maxSpeed = maxSpeedAchieved;
            avgSpeed = avgSpeedAchieved;
            zones = zoneInfo;
        }
    }
}