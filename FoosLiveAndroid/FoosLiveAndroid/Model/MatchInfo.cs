﻿namespace FoosLiveAndroid.Model
{
    public static class MatchInfo
    {
        public static string Team1Name { get; set; }
        public static string Team2Name { get; set; }
        public static int Team1Score { get; set; }
        public static int Team2Score { get; set; }
        public static double MaxSpeed { get; set; }
        public static double AvgSpeed { get; set; }
        public static string Duration { get; set; }
        public static int[] Zones { get; set; }

        public static void SetUp(string team1Name, int team1Score, string team2Name, int team2Score,
                            double maxSpeedAchieved, double avgSpeedAchieved, int[] zoneInfo, string duration)
        {
            // Assign team one info
            Team1Name = team1Name;
            Team1Score = team1Score;

            // Assign team two info
            Team2Name = team2Name;
            Team2Score = team2Score;

            // Assign match info
            MaxSpeed = maxSpeedAchieved;
            AvgSpeed = avgSpeedAchieved;
            Zones = zoneInfo;

            Duration = duration;
        }
    }
}