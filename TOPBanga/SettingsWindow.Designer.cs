namespace TOPBanga
{
    partial class SettingsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.redTeamGoalLabel = new System.Windows.Forms.Label();
            this.redTeamGoalSet = new System.Windows.Forms.Button();
            this.redTeamWinLabel = new System.Windows.Forms.Label();
            this.redTeamWinSet = new System.Windows.Forms.Button();
            this.blueTeamGoalLabel = new System.Windows.Forms.Label();
            this.blueTeamGoalSet = new System.Windows.Forms.Button();
            this.blueTeamWinLabel = new System.Windows.Forms.Label();
            this.blueTeamWinSet = new System.Windows.Forms.Button();
            this.ballLostLabel = new System.Windows.Forms.Label();
            this.ballLostSet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // redTeamGoalLabel
            // 
            this.redTeamGoalLabel.AutoSize = true;
            this.redTeamGoalLabel.Location = new System.Drawing.Point(34, 53);
            this.redTeamGoalLabel.Name = "redTeamGoalLabel";
            this.redTeamGoalLabel.Size = new System.Drawing.Size(82, 13);
            this.redTeamGoalLabel.TabIndex = 0;
            this.redTeamGoalLabel.Text = "Red Team Goal";
            // 
            // redTeamGoalSet
            // 
            this.redTeamGoalSet.Location = new System.Drawing.Point(149, 48);
            this.redTeamGoalSet.Name = "redTeamGoalSet";
            this.redTeamGoalSet.Size = new System.Drawing.Size(75, 23);
            this.redTeamGoalSet.TabIndex = 1;
            this.redTeamGoalSet.Text = "Set";
            this.redTeamGoalSet.UseVisualStyleBackColor = true;
            this.redTeamGoalSet.Click += new System.EventHandler(this.redTeamGoalSet_Click);
            // 
            // redTeamWinLabel
            // 
            this.redTeamWinLabel.AutoSize = true;
            this.redTeamWinLabel.Location = new System.Drawing.Point(34, 91);
            this.redTeamWinLabel.Name = "redTeamWinLabel";
            this.redTeamWinLabel.Size = new System.Drawing.Size(84, 13);
            this.redTeamWinLabel.TabIndex = 2;
            this.redTeamWinLabel.Text = "Red Team Wins";
            // 
            // redTeamWinSet
            // 
            this.redTeamWinSet.Location = new System.Drawing.Point(149, 86);
            this.redTeamWinSet.Name = "redTeamWinSet";
            this.redTeamWinSet.Size = new System.Drawing.Size(75, 23);
            this.redTeamWinSet.TabIndex = 3;
            this.redTeamWinSet.Text = "Set";
            this.redTeamWinSet.UseVisualStyleBackColor = true;
            this.redTeamWinSet.Click += new System.EventHandler(this.redTeamWinSet_Click);
            // 
            // blueTeamGoalLabel
            // 
            this.blueTeamGoalLabel.AutoSize = true;
            this.blueTeamGoalLabel.Location = new System.Drawing.Point(330, 53);
            this.blueTeamGoalLabel.Name = "blueTeamGoalLabel";
            this.blueTeamGoalLabel.Size = new System.Drawing.Size(83, 13);
            this.blueTeamGoalLabel.TabIndex = 4;
            this.blueTeamGoalLabel.Text = "Blue Team Goal";
            // 
            // blueTeamGoalSet
            // 
            this.blueTeamGoalSet.Location = new System.Drawing.Point(445, 48);
            this.blueTeamGoalSet.Name = "blueTeamGoalSet";
            this.blueTeamGoalSet.Size = new System.Drawing.Size(75, 23);
            this.blueTeamGoalSet.TabIndex = 5;
            this.blueTeamGoalSet.Text = "Set";
            this.blueTeamGoalSet.UseVisualStyleBackColor = true;
            this.blueTeamGoalSet.Click += new System.EventHandler(this.blueTeamGoalSet_Click);
            // 
            // blueTeamWinLabel
            // 
            this.blueTeamWinLabel.AutoSize = true;
            this.blueTeamWinLabel.Location = new System.Drawing.Point(330, 91);
            this.blueTeamWinLabel.Name = "blueTeamWinLabel";
            this.blueTeamWinLabel.Size = new System.Drawing.Size(85, 13);
            this.blueTeamWinLabel.TabIndex = 6;
            this.blueTeamWinLabel.Text = "Blue Team Wins";
            // 
            // blueTeamWinSet
            // 
            this.blueTeamWinSet.Location = new System.Drawing.Point(445, 86);
            this.blueTeamWinSet.Name = "blueTeamWinSet";
            this.blueTeamWinSet.Size = new System.Drawing.Size(75, 23);
            this.blueTeamWinSet.TabIndex = 7;
            this.blueTeamWinSet.Text = "Set";
            this.blueTeamWinSet.UseVisualStyleBackColor = true;
            this.blueTeamWinSet.Click += new System.EventHandler(this.blueTeamWinSet_Click);
            // 
            // ballLostLabel
            // 
            this.ballLostLabel.AutoSize = true;
            this.ballLostLabel.Location = new System.Drawing.Point(227, 162);
            this.ballLostLabel.Name = "ballLostLabel";
            this.ballLostLabel.Size = new System.Drawing.Size(47, 13);
            this.ballLostLabel.TabIndex = 8;
            this.ballLostLabel.Text = "Ball Lost";
            // 
            // ballLostSet
            // 
            this.ballLostSet.Location = new System.Drawing.Point(302, 157);
            this.ballLostSet.Name = "ballLostSet";
            this.ballLostSet.Size = new System.Drawing.Size(75, 23);
            this.ballLostSet.TabIndex = 9;
            this.ballLostSet.Text = "Set";
            this.ballLostSet.UseVisualStyleBackColor = true;
            this.ballLostSet.Click += new System.EventHandler(this.ballLostSet_Click);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 369);
            this.Controls.Add(this.ballLostSet);
            this.Controls.Add(this.ballLostLabel);
            this.Controls.Add(this.blueTeamWinSet);
            this.Controls.Add(this.blueTeamWinLabel);
            this.Controls.Add(this.blueTeamGoalSet);
            this.Controls.Add(this.blueTeamGoalLabel);
            this.Controls.Add(this.redTeamWinSet);
            this.Controls.Add(this.redTeamWinLabel);
            this.Controls.Add(this.redTeamGoalSet);
            this.Controls.Add(this.redTeamGoalLabel);
            this.Name = "SettingsWindow";
            this.Text = "SettingsWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label redTeamGoalLabel;
        private System.Windows.Forms.Button redTeamGoalSet;
        private System.Windows.Forms.Label redTeamWinLabel;
        private System.Windows.Forms.Button redTeamWinSet;
        private System.Windows.Forms.Label blueTeamGoalLabel;
        private System.Windows.Forms.Button blueTeamGoalSet;
        private System.Windows.Forms.Label blueTeamWinLabel;
        private System.Windows.Forms.Button blueTeamWinSet;
        private System.Windows.Forms.Label ballLostLabel;
        private System.Windows.Forms.Button ballLostSet;
    }
}