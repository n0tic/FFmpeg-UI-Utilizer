using FFmpeg_Utilizer.Data;
using FFmpeg_Utilizer.Modules;
using FFMPEG_Utilizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFmpeg_Utilizer
{
    public partial class Main : Form
    {
        //Settings
        internal Settings settings = new Settings();

        //Modules
        public NoticeModule notice;

        public Main()
        {
            InitializeComponent();

            //Init Modules
            notice = new NoticeModule(this);

            SetupSoftware();
        }

        private void SetupSoftware()
        {
            notice.CloseNotice();
            AddTabs();
            Core.ChangeTab(0);

            SoftwareLabel.Text = Core.softwareName + " " + Core.GetVersion();
            AuthorLabel.Text = Core.authorRealName + " AKA " + Core.authorName;
            GitLabel.Text = Core.softwareGIT;
        }





        #region Software Window
        private void NoticeCloseButton_Click(object sender, EventArgs e) => notice.CloseNotice();

        private void ApplicationCloseButton_Click(object sender, EventArgs e) => Application.Exit();

        private void ApplicationMinimizeButton_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        private void TopLogo_MouseDown(object sender, MouseEventArgs e) => Core.MoveWindow(this, e);
        #endregion Software Window

        #region Tabs
        private void AddTabs()
        {
            Core.AddTab(Menu_EncoderTab, Menu_EncoderTabIndicator, Menu_EncoderTabLabel, EncoderMainPanel);
            Core.AddTab(Menu_CutMergeTab, Menu_CutMergeTabIndicator, Menu_CutMergeTabLabel, null);
            Core.AddTab(Menu_MergeTab, Menu_MergeTabIndicator, Menu_MergeTabLabel, MergeMainPanel);
            Core.AddTab(Menu_CutTab, Menu_CutTabIndicator, Menu_CutTabLabel, CutMainPanel);
            Core.AddTab(Menu_M3U8Tab, Menu_M3U8TabIndicator, Menu_M3U8TabLabel, M3U8MainPanel);
            Core.AddTab(Menu_ArgumentsTab, Menu_ArgumentsTabIndicator, Menu_ArgumentsTabLabel, ArgumentMainPanel);
            Core.AddTab(Menu_SettingsTab, Menu_SettingsTabIndicator, Menu_SettingsTabLabel, SettingsMainPanel);
            Core.AddTab(Menu_UpdatesTab, Menu_UpdatesTabIndicator, Menu_UpdatesTabLabel, UpdateMainPanel);
        }

        private void Menu_EncoderTabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.Encoder);

        private void Menu_CutMergeTabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.CutMerge);

        private void Menu_CutTabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.Cut);

        private void Menu_MergeTabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.Merge);

        private void Menu_M3U8TabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.M3U8);

        private void Menu_ArgumentsTabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.Argument);

        private void Menu_SettingsTabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.Settings);

        private void Menu_UpdatesTabIndicator_Click(object sender, EventArgs e) => Core.ChangeTab(Core.Tabs.Updater);
        #endregion Tabs

        #region Testing
        private void Button1_Click(object sender, EventArgs e)
        {
            notice.SetNotice("A notice system for errors, warnings, infos and successful operations.", NoticeModule.TypeNotice.Warning);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            notice.SetNotice("A notice system for errors, warnings, infos and successful operations.", NoticeModule.TypeNotice.Info);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            notice.SetNotice("A notice system for errors, warnings, infos and successful operations.", NoticeModule.TypeNotice.Error);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            notice.SetNotice("A notice system for errors, warnings, infos and successful operations.", NoticeModule.TypeNotice.Success);
        }
        #endregion Testing
    }
}
