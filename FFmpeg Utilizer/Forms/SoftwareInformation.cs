using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Forms
{
    public partial class SoftwareInformation : Form
    {
        public SoftwareInformation()
        {
            InitializeComponent();

            SoftwareNameLabel.Text = Core.softwareName;
            SoftwareVersionLabel.Text = Core.GetVersion();
            SoftwareAuthorLabel.Text = Core.authorRealName + " AKA " + Core.authorName;
            SoftwareCompanyWebsite.Text = Core.companyWebsite;
            SoftwareGithubRepoLabel.Text = Core.softwareGIT;
            ContactLabel.Text = Core.authorContact;
        }

        private void TopPanel_MouseDown(object sender, MouseEventArgs e) => Core.MoveWindow(this, e);

        private void CloseButton_Click(object sender, EventArgs e) => Close();

        private void SoftwareCompanyWebsite_Click(object sender, EventArgs e) => Process.Start(Core.companyWebsite);

        private void SoftwareGithubRepoLabel_Click(object sender, EventArgs e) => Process.Start(Core.softwareGITURL);

        private void PackURL_Click(object sender, EventArgs e) => Process.Start("https://gumroad.com/l/PKAHx");

        private void PackCreator_Click(object sender, EventArgs e) => Process.Start("https://gumroad.com/darkwing");
    }
}
