namespace FFmpeg_Utilizer.Forms
{
    partial class SoftwareInformation
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
            this.TopPanel = new System.Windows.Forms.Panel();
            this.ApplicationMinimizeButton = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Label();
            this.TopLogo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ContactLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SoftwareGithubRepoLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SoftwareCompanyWebsite = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SoftwareAuthorLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SoftwareVersionLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SoftwareNameLabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.PackCreator = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PackURL = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.TopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TopLogo)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.BackColor = System.Drawing.Color.White;
            this.TopPanel.Controls.Add(this.ApplicationMinimizeButton);
            this.TopPanel.Controls.Add(this.CloseButton);
            this.TopPanel.Controls.Add(this.TopLogo);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(344, 27);
            this.TopPanel.TabIndex = 3;
            this.TopPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopPanel_MouseDown);
            // 
            // ApplicationMinimizeButton
            // 
            this.ApplicationMinimizeButton.BackColor = System.Drawing.Color.White;
            this.ApplicationMinimizeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ApplicationMinimizeButton.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ApplicationMinimizeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(64)))), ((int)(((byte)(82)))));
            this.ApplicationMinimizeButton.Location = new System.Drawing.Point(1030, -4);
            this.ApplicationMinimizeButton.Name = "ApplicationMinimizeButton";
            this.ApplicationMinimizeButton.Size = new System.Drawing.Size(24, 27);
            this.ApplicationMinimizeButton.TabIndex = 4;
            this.ApplicationMinimizeButton.Text = "_";
            this.ApplicationMinimizeButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.White;
            this.CloseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.CloseButton.Font = new System.Drawing.Font("Bahnschrift Condensed", 14.25F, System.Drawing.FontStyle.Bold);
            this.CloseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(64)))), ((int)(((byte)(82)))));
            this.CloseButton.Location = new System.Drawing.Point(320, 0);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(24, 27);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "X";
            this.CloseButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // TopLogo
            // 
            this.TopLogo.Dock = System.Windows.Forms.DockStyle.Left;
            this.TopLogo.Image = global::FFmpeg_Utilizer.Properties.Resources.ffmpegUtilizerLogo;
            this.TopLogo.Location = new System.Drawing.Point(0, 0);
            this.TopLogo.Name = "TopLogo";
            this.TopLogo.Size = new System.Drawing.Size(200, 27);
            this.TopLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.TopLogo.TabIndex = 0;
            this.TopLogo.TabStop = false;
            this.TopLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopPanel_MouseDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.panel1.Controls.Add(this.PackURL);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.PackCreator);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.ContactLabel);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.SoftwareGithubRepoLabel);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.SoftwareCompanyWebsite);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.SoftwareAuthorLabel);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.SoftwareVersionLabel);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.SoftwareNameLabel);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.TopPanel);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 183);
            this.panel1.TabIndex = 4;
            // 
            // ContactLabel
            // 
            this.ContactLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ContactLabel.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.ContactLabel.Location = new System.Drawing.Point(124, 114);
            this.ContactLabel.Name = "ContactLabel";
            this.ContactLabel.Size = new System.Drawing.Size(217, 16);
            this.ContactLabel.TabIndex = 89;
            this.ContactLabel.Text = "-";
            this.ContactLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 16);
            this.label2.TabIndex = 88;
            this.label2.Text = "Contact:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SoftwareGithubRepoLabel
            // 
            this.SoftwareGithubRepoLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SoftwareGithubRepoLabel.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.SoftwareGithubRepoLabel.Location = new System.Drawing.Point(124, 98);
            this.SoftwareGithubRepoLabel.Name = "SoftwareGithubRepoLabel";
            this.SoftwareGithubRepoLabel.Size = new System.Drawing.Size(217, 16);
            this.SoftwareGithubRepoLabel.TabIndex = 87;
            this.SoftwareGithubRepoLabel.Text = "-";
            this.SoftwareGithubRepoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SoftwareGithubRepoLabel.Click += new System.EventHandler(this.SoftwareGithubRepoLabel_Click);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 16);
            this.label9.TabIndex = 86;
            this.label9.Text = "Github Repository:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SoftwareCompanyWebsite
            // 
            this.SoftwareCompanyWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SoftwareCompanyWebsite.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.SoftwareCompanyWebsite.Location = new System.Drawing.Point(124, 81);
            this.SoftwareCompanyWebsite.Name = "SoftwareCompanyWebsite";
            this.SoftwareCompanyWebsite.Size = new System.Drawing.Size(217, 16);
            this.SoftwareCompanyWebsite.TabIndex = 85;
            this.SoftwareCompanyWebsite.Text = "-";
            this.SoftwareCompanyWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SoftwareCompanyWebsite.Click += new System.EventHandler(this.SoftwareCompanyWebsite_Click);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 80);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 16);
            this.label7.TabIndex = 84;
            this.label7.Text = "Company Website:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SoftwareAuthorLabel
            // 
            this.SoftwareAuthorLabel.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.SoftwareAuthorLabel.Location = new System.Drawing.Point(124, 64);
            this.SoftwareAuthorLabel.Name = "SoftwareAuthorLabel";
            this.SoftwareAuthorLabel.Size = new System.Drawing.Size(217, 16);
            this.SoftwareAuthorLabel.TabIndex = 83;
            this.SoftwareAuthorLabel.Text = "-";
            this.SoftwareAuthorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 16);
            this.label5.TabIndex = 82;
            this.label5.Text = "Software Author:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SoftwareVersionLabel
            // 
            this.SoftwareVersionLabel.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.SoftwareVersionLabel.Location = new System.Drawing.Point(124, 47);
            this.SoftwareVersionLabel.Name = "SoftwareVersionLabel";
            this.SoftwareVersionLabel.Size = new System.Drawing.Size(217, 16);
            this.SoftwareVersionLabel.TabIndex = 81;
            this.SoftwareVersionLabel.Text = "-";
            this.SoftwareVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 16);
            this.label3.TabIndex = 80;
            this.label3.Text = "Software Version:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SoftwareNameLabel
            // 
            this.SoftwareNameLabel.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.SoftwareNameLabel.Location = new System.Drawing.Point(124, 31);
            this.SoftwareNameLabel.Name = "SoftwareNameLabel";
            this.SoftwareNameLabel.Size = new System.Drawing.Size(217, 16);
            this.SoftwareNameLabel.TabIndex = 79;
            this.SoftwareNameLabel.Text = "-";
            this.SoftwareNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(3, 30);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(115, 16);
            this.label17.TabIndex = 78;
            this.label17.Text = "Software Name:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PackCreator
            // 
            this.PackCreator.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PackCreator.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.PackCreator.Location = new System.Drawing.Point(124, 145);
            this.PackCreator.Name = "PackCreator";
            this.PackCreator.Size = new System.Drawing.Size(217, 16);
            this.PackCreator.TabIndex = 91;
            this.PackCreator.Text = "Alexandr Martinov";
            this.PackCreator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.PackCreator.Click += new System.EventHandler(this.PackCreator_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 16);
            this.label4.TabIndex = 90;
            this.label4.Text = "Icon Pack Creator:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PackURL
            // 
            this.PackURL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PackURL.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.5F);
            this.PackURL.Location = new System.Drawing.Point(124, 162);
            this.PackURL.Name = "PackURL";
            this.PackURL.Size = new System.Drawing.Size(209, 16);
            this.PackURL.TabIndex = 93;
            this.PackURL.Text = "https://gumroad.com/l/PKAHx";
            this.PackURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.PackURL.Click += new System.EventHandler(this.PackURL_Click);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Bahnschrift Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 16);
            this.label8.TabIndex = 92;
            this.label8.Text = "Pack URL:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SoftwareInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(124)))), ((int)(((byte)(237)))));
            this.ClientSize = new System.Drawing.Size(346, 185);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoftwareInformation";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SoftwareInformation";
            this.TopMost = true;
            this.TopPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TopLogo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox TopLogo;
        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Label ApplicationMinimizeButton;
        private System.Windows.Forms.Label CloseButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label SoftwareNameLabel;
        private System.Windows.Forms.Label SoftwareAuthorLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label SoftwareVersionLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label SoftwareGithubRepoLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label SoftwareCompanyWebsite;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label ContactLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label PackCreator;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label PackURL;
        private System.Windows.Forms.Label label8;
    }
}