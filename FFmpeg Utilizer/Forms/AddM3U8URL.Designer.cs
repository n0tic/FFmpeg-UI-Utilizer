namespace FFmpeg_Utilizer.Forms
{
    partial class AddM3U8URL
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
            this._CancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.NameField = new System.Windows.Forms.TextBox();
            this.URLField = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.TopPanel.Size = new System.Drawing.Size(497, 27);
            this.TopPanel.TabIndex = 2;
            this.TopPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopLogo_MouseDown);
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
            this.CloseButton.Font = new System.Drawing.Font("Bahnschrift Condensed", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(64)))), ((int)(((byte)(82)))));
            this.CloseButton.Location = new System.Drawing.Point(473, 0);
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
            this.TopLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopLogo_MouseDown);
            // 
            // _CancelButton
            // 
            this._CancelButton.BackColor = System.Drawing.Color.White;
            this._CancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this._CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._CancelButton.Font = new System.Drawing.Font("Bahnschrift SemiBold SemiConden", 9F, System.Drawing.FontStyle.Bold);
            this._CancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._CancelButton.Location = new System.Drawing.Point(12, 64);
            this._CancelButton.Name = "_CancelButton";
            this._CancelButton.Size = new System.Drawing.Size(117, 24);
            this._CancelButton.TabIndex = 74;
            this._CancelButton.Text = "Cancel";
            this._CancelButton.UseVisualStyleBackColor = false;
            this._CancelButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.BackColor = System.Drawing.Color.White;
            this.OKButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(172)))), ((int)(((byte)(229)))));
            this.OKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OKButton.Font = new System.Drawing.Font("Bahnschrift SemiBold SemiConden", 9F, System.Drawing.FontStyle.Bold);
            this.OKButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.OKButton.Location = new System.Drawing.Point(370, 64);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(117, 24);
            this.OKButton.TabIndex = 73;
            this.OKButton.Text = "Add HLS/M3U8 URL";
            this.OKButton.UseVisualStyleBackColor = false;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // NameField
            // 
            this.NameField.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.NameField.Location = new System.Drawing.Point(12, 36);
            this.NameField.Name = "NameField";
            this.NameField.Size = new System.Drawing.Size(188, 23);
            this.NameField.TabIndex = 75;
            // 
            // URLField
            // 
            this.URLField.Font = new System.Drawing.Font("Bahnschrift", 9.75F);
            this.URLField.Location = new System.Drawing.Point(206, 36);
            this.URLField.Name = "URLField";
            this.URLField.Size = new System.Drawing.Size(281, 23);
            this.URLField.TabIndex = 76;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.panel1.Controls.Add(this.TopPanel);
            this.panel1.Location = new System.Drawing.Point(1, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(497, 98);
            this.panel1.TabIndex = 77;
            // 
            // AddM3U8URL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(124)))), ((int)(((byte)(237)))));
            this.ClientSize = new System.Drawing.Size(499, 100);
            this.Controls.Add(this.URLField);
            this.Controls.Add(this.NameField);
            this.Controls.Add(this._CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddM3U8URL";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddM3U8URL";
            this.TopMost = true;
            this.TopPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TopLogo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Label ApplicationMinimizeButton;
        private System.Windows.Forms.Label CloseButton;
        private System.Windows.Forms.PictureBox TopLogo;
        private System.Windows.Forms.Button _CancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.TextBox NameField;
        internal System.Windows.Forms.TextBox URLField;
    }
}