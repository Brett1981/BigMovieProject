namespace MovieDB_Windows_app.Views
{
    partial class ViewMovie
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
            this.ipLabel = new System.Windows.Forms.Label();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.dirTextBox = new System.Windows.Forms.TextBox();
            this.dirLabel = new System.Windows.Forms.Label();
            this.playMovieButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(10, 15);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(17, 13);
            this.ipLabel.TabIndex = 0;
            this.ipLabel.Text = "IP";
            // 
            // ipTextBox
            // 
            this.ipTextBox.Location = new System.Drawing.Point(51, 12);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(167, 20);
            this.ipTextBox.TabIndex = 1;
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(51, 38);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(167, 20);
            this.portTextBox.TabIndex = 3;
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(10, 41);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(26, 13);
            this.portLabel.TabIndex = 2;
            this.portLabel.Text = "Port";
            // 
            // dirTextBox
            // 
            this.dirTextBox.Location = new System.Drawing.Point(51, 64);
            this.dirTextBox.Name = "dirTextBox";
            this.dirTextBox.Size = new System.Drawing.Size(167, 20);
            this.dirTextBox.TabIndex = 5;
            // 
            // dirLabel
            // 
            this.dirLabel.AutoSize = true;
            this.dirLabel.Location = new System.Drawing.Point(10, 67);
            this.dirLabel.Name = "dirLabel";
            this.dirLabel.Size = new System.Drawing.Size(20, 13);
            this.dirLabel.TabIndex = 4;
            this.dirLabel.Text = "Dir";
            // 
            // playMovieButton
            // 
            this.playMovieButton.Location = new System.Drawing.Point(81, 99);
            this.playMovieButton.Name = "playMovieButton";
            this.playMovieButton.Size = new System.Drawing.Size(75, 23);
            this.playMovieButton.TabIndex = 6;
            this.playMovieButton.Text = "Play movie";
            this.playMovieButton.UseVisualStyleBackColor = true;
            this.playMovieButton.Click += new System.EventHandler(this.playMovieButton_Click);
            // 
            // ViewMovie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 134);
            this.Controls.Add(this.playMovieButton);
            this.Controls.Add(this.dirTextBox);
            this.Controls.Add(this.dirLabel);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.ipLabel);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(258, 173);
            this.MinimumSize = new System.Drawing.Size(258, 173);
            this.Name = "ViewMovie";
            this.Text = "Watch settings";
            this.Load += new System.EventHandler(this.ViewMovieSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.TextBox dirTextBox;
        private System.Windows.Forms.Label dirLabel;
        private System.Windows.Forms.Button playMovieButton;
    }
}