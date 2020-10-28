using System;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Forms
{
    public partial class AddM3U8URL : Form
    {
        public AddM3U8URL()
        {
            InitializeComponent();

            NameField.Text = "Name...";
            NameField.GotFocus += NameField_GotFocus;
            NameField.LostFocus += NameField_LostFocus;

            URLField.Text = "URL...";
            URLField.GotFocus += URLField_GotFocus; ;
            URLField.LostFocus += URLField_LostFocus; ;
        }

        private void URLField_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(URLField.Text))
                URLField.Text = "URL...";
        }

        private void URLField_GotFocus(object sender, EventArgs e)
        {
            if (URLField.Text == "URL...")
                URLField.Text = "";
        }

        private void NameField_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameField.Text))
                NameField.Text = "Name...";
        }

        private void NameField_GotFocus(object sender, EventArgs e)
        {
            if (NameField.Text == "Name...")
                NameField.Text = "";
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TopLogo_MouseDown(object sender, MouseEventArgs e) => Core.MoveWindow(this, e);

        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}