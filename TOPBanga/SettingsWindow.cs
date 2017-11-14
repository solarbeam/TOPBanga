using System;
using System.Windows.Forms;
using TOPBanga.Util;
using TOPBanga.Util.SoundPlayers;

namespace TOPBanga
{
    public partial class SettingsWindow : Form
    {
        private SoundAlerts alerts;

        private String dialogFilter = "Wav file|*.wav";

        public SettingsWindow(SoundAlerts alerts)
        {
            InitializeComponent();

            this.alerts = alerts;
        }
        private String getFilePath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = dialogFilter;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
                return null;
        }

        private void redTeamGoalSet_Click(object sender, EventArgs e)
        {
            this.alerts.redTeamGoal = new PlayerWAV();
            this.alerts.redTeamGoal.SetPath(getFilePath());
        }

        private void redTeamWinSet_Click(object sender, EventArgs e)
        {
            this.alerts.redTeamWins = new PlayerWAV();
            this.alerts.redTeamWins.SetPath(getFilePath());
        }

        private void blueTeamGoalSet_Click(object sender, EventArgs e)
        {
            this.alerts.blueTeamGoal = new PlayerWAV();
            this.alerts.blueTeamGoal.SetPath(getFilePath());
        }

        private void blueTeamWinSet_Click(object sender, EventArgs e)
        {
            this.alerts.redTeamWins = new PlayerWAV();
            this.alerts.redTeamWins.SetPath(getFilePath());
        }

        private void ballLostSet_Click(object sender, EventArgs e)
        {
            this.alerts.ballLost = new PlayerWAV();
            this.alerts.ballLost.SetPath(getFilePath());
        }
    }
}
