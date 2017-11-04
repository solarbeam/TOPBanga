using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TOPBanga.Util.SoundPlayers;

namespace TOPBanga
{
    public partial class SettingsWindow : Form
    {
        private VideoFromFile previousForm;
        private String dialogFilter = "Wav file|*.wav";
        public SettingsWindow(VideoFromFile previousForm)
        {
            InitializeComponent();
            this.previousForm = previousForm;
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
            this.previousForm.alerts.redTeamGoal = new PlayerWAV();
            this.previousForm.alerts.redTeamGoal.SetPath(getFilePath());
        }

        private void redTeamWinSet_Click(object sender, EventArgs e)
        {
            this.previousForm.alerts.redTeamWins = new PlayerWAV();
            this.previousForm.alerts.redTeamWins.SetPath(getFilePath());
        }

        private void blueTeamGoalSet_Click(object sender, EventArgs e)
        {
            this.previousForm.alerts.blueTeamGoal = new PlayerWAV();
            this.previousForm.alerts.blueTeamGoal.SetPath(getFilePath());
        }

        private void blueTeamWinSet_Click(object sender, EventArgs e)
        {
            this.previousForm.alerts.redTeamWins = new PlayerWAV();
            this.previousForm.alerts.redTeamWins.SetPath(getFilePath());
        }

        private void ballLostSet_Click(object sender, EventArgs e)
        {
            this.previousForm.alerts.ballLost = new PlayerWAV();
            this.previousForm.alerts.ballLost.SetPath(getFilePath());
        }
    }
}
