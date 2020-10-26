#pragma warning disable IDE0044 // Nagging about hasInternet

using FFmpeg_Utilizer.Data;
using FFmpeg_Utilizer.Modules;
using FFMPEG_Utilizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
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
        public UtilityUpdaterModule updater;

        bool hasInternet = true;

        public Main()
        {
            InitializeComponent();

            SetupSoftware();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if(hasInternet)
                updater.StartUpdateCheckAsync();

            SetupFolders();
            SetupUI();
        }

        private void SetupSoftware()
        {
            hasInternet = Core.IsInternetConnectionAvailable();

            // SecurityProtocol SSL/TSL
            ServicePointManager.Expect100Continue = true; // Not sure if this is needed?
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SetupModules();
            InternetFeatures(); // Enables network modules too
        }

        private void SetupModules()
        {
            //Init Modules
            notice = new NoticeModule(this);
            notice.CloseNotice();

            if(hasInternet)
                updater = new UtilityUpdaterModule(this);
        }

        private void InternetFeatures()
        {
            if (!hasInternet)
            {
                hasInternet = false;
                M3U8MainPanel.Dispose();
                Settings_NetPanel.Dispose();
                UpdateMainPanel.Dispose();
                notice.SetNotice("Network features disabled. No network connection detected.", NoticeModule.TypeNotice.Info);
            }
        }

        private void SetupFolders()
        {
            //Check if default directory exist.
            if (!Core.DoesAllDefaultDirectoryExist())
                Core.CreateAllDefaultFolders();
        }

        private void SetupUI()
        {
            //Tab System
            AddTabs();
            Core.ChangeTab(0);

            #region Settings

            //Get Settings Data
            foreach (Libs.Overwrite ow in (Libs.Overwrite[])Enum.GetValues(typeof(Libs.Overwrite)))
                Settings_OverwriteDropdown.Items.Add(ow);

            foreach (Libs.VCodec codec in (Libs.VCodec[])Enum.GetValues(typeof(Libs.VCodec)))
                Settings_VideoCodecDropdown.Items.Add(codec);

            foreach (Libs.ACodec codec in (Libs.ACodec[])Enum.GetValues(typeof(Libs.ACodec)))
                Settings_AudioCodecDropdown.Items.Add(codec);

            foreach (Libs.Preset quality in (Libs.Preset[])Enum.GetValues(typeof(Libs.Preset)))
                Settings_QualityDropdown.Items.Add(quality);

            if (settings.loaded)
            {
                if (File.Exists(settings.ffmpegPath)) Settings_FFmpegPathBox.Text = settings.ffmpegPath;
                else if(File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg)))
                {
                    settings.ffmpegPath = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg);
                    Settings_FFmpegPathBox.Text = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg);
                }

                if (File.Exists(settings.ffplayPath)) Settings_FFplayPathBox.Text = settings.ffplayPath;
                else if (File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay)))
                {
                    settings.ffplayPath = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay);
                    Settings_FFplayPathBox.Text = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay);
                }

                if (Directory.Exists(settings.outputLocation)) Settings_DefaultOutputPathBox.Text = settings.outputLocation;
                else if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                {
                    settings.outputLocation = Core.GetSubfolder(Core.SubFolders.Output);
                    Settings_DefaultOutputPathBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
                }

                Settings_OverwriteDropdown.SelectedIndex = Settings_OverwriteDropdown.FindStringExact(settings.overwrite.ToString());
                Settings_VideoCodecDropdown.SelectedIndex = Settings_VideoCodecDropdown.FindStringExact(settings.vCodec.ToString());
                Settings_AudioCodecDropdown.SelectedIndex = Settings_AudioCodecDropdown.FindStringExact(settings.aCodec.ToString());
                Settings_QualityDropdown.SelectedIndex = Settings_QualityDropdown.FindStringExact(settings.quality.ToString());

                if (hasInternet)
                {
                    Settings_InstalledVersionLabel.Text = settings.ffVersion;
                    Settings_LatestUpdateLabel.Text = settings.lastUpdate.ToString();

                    Update_InstalledVersionLabel.Text = settings.ffVersion;
                    Update_LatestUpdateLabel.Text = settings.lastUpdate.ToString();

                    Settings_OnlineVerLabel.Text = settings.nextUpdate;
                }
            }
            else //Default settings
            {
                if (!Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output));

                Settings_DefaultOutputPathBox.Text = Core.GetSubfolder(Core.SubFolders.Output);

                Settings_OverwriteDropdown.SelectedIndex = 0;
                Settings_VideoCodecDropdown.SelectedIndex = 2;
                Settings_AudioCodecDropdown.SelectedIndex = 2;
                Settings_QualityDropdown.SelectedIndex = 0;
            }

            #endregion Settings



            //Set lower left information
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
            //Set reference;
            Core.main = this;
            Core.AddTab(Menu_EncoderTab, Menu_EncoderTabIndicator, Menu_EncoderTabLabel, EncoderMainPanel);
            Core.AddTab(Menu_CutMergeTab, Menu_CutMergeTabIndicator, Menu_CutMergeTabLabel, null);
            Core.AddTab(Menu_MergeTab, Menu_MergeTabIndicator, Menu_MergeTabLabel, MergeMainPanel);
            Core.AddTab(Menu_CutTab, Menu_CutTabIndicator, Menu_CutTabLabel, CutMainPanel);
            Core.AddTab(Menu_M3U8Tab, Menu_M3U8TabIndicator, Menu_M3U8TabLabel, M3U8MainPanel, hasInternet);
            Core.AddTab(Menu_ArgumentsTab, Menu_ArgumentsTabIndicator, Menu_ArgumentsTabLabel, ArgumentMainPanel);
            Core.AddTab(Menu_SettingsTab, Menu_SettingsTabIndicator, Menu_SettingsTabLabel, SettingsMainPanel);
            Core.AddTab(Menu_UpdatesTab, Menu_UpdatesTabIndicator, Menu_UpdatesTabLabel, UpdateMainPanel, hasInternet);
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

        private void Update_CheckForUpdateButton_Click(object sender, EventArgs e)
        {
            updater.StartUpdateCheckAsync();
        }

        private void Update_DownloadButton_Click(object sender, EventArgs e) => updater.StartUpdate();

        private void Settings_AutoDefaultOutputButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output))) Directory.CreateDirectory(Core.GetSubfolder(Core.SubFolders.Output));

            settings.outputLocation = Core.GetSubfolder(Core.SubFolders.Output);
            Settings_DefaultOutputPathBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
        }

        private void Settings_SaveButton_Click(object sender, EventArgs e)
        {
            settings.ffmpegPath = Settings_FFmpegPathBox.Text;
            settings.ffplayPath = Settings_FFplayPathBox.Text;
            if (Directory.Exists(Settings_DefaultOutputPathBox.Text)) settings.outputLocation = Settings_DefaultOutputPathBox.Text;

            settings.overwrite = (Libs.Overwrite)Enum.Parse(typeof(Libs.Overwrite), Settings_OverwriteDropdown.Text);
            settings.vCodec = (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), Settings_VideoCodecDropdown.Text);
            settings.aCodec = (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), Settings_AudioCodecDropdown.Text);
            settings.quality = (Libs.Preset)Enum.Parse(typeof(Libs.Preset), Settings_QualityDropdown.Text);
            settings.hideConsole = Settings_HideConsoleCheckbox.Checked;

            settings.URIPort = Convert.ToInt32(Settings_URIServerPort.Value);
            settings.URIautoStart = Settings_URIServerAutoStart.Checked;

            settings.SaveSettings();
            notice.SetNotice("Settings has been saved.", NoticeModule.TypeNotice.Success);
        }

        private void Settings_ResetButton_Click(object sender, EventArgs e)
        {
            Settings_FFmpegPathBox.Text = "";
            Settings_FFplayPathBox.Text = "";
            Settings_DefaultOutputPathBox.Text = "";

            Settings_OverwriteDropdown.SelectedIndex = 0;
            Settings_VideoCodecDropdown.SelectedIndex = 2;
            Settings_AudioCodecDropdown.SelectedIndex = 2;
            Settings_QualityDropdown.SelectedIndex = 0;
            Settings_HideConsoleCheckbox.Checked = false;

            Settings_InstalledVersionLabel.Text = "";
            Settings_LatestUpdateLabel.Text = "";

            Settings_URIServerPort.Value = 288;
            Settings_URIServerAutoStart.Checked = false;

            settings.ResetSettings();
            settings.SaveSettings();

            Settings_DefaultOutputPathBox.Text = settings.outputLocation;
        }

        private void Settings_DownloadButton_Click(object sender, EventArgs e)
        {
            updater.StartUpdate();
            Core.ChangeTab(Core.Tabs.Updater);
        }
    }
}
