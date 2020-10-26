#pragma warning disable IDE0044 // Nagging about hasInternet

using FFmpeg_Utilizer.Data;
using FFmpeg_Utilizer.Modules;
using FFMPEG_Utilizer;
using FFMPEG_Utilizer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public EncodingProcesser encodingProcesser;
        public UriRequestsHandler uriRequestHandler;

        //FFplay process
        private Process PlayProcess;

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
            encodingProcesser = new EncodingProcesser(this);

            if (hasInternet)
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

                Settings_HideConsoleCheckbox.Checked = settings.hideConsole;

                Settings_URIServerPort.Value = Convert.ToDecimal(settings.URIPort);
                if(settings.URIautoStart)
                {
                    Settings_URIServerAutoStart.Checked = settings.URIautoStart;
                    Settings_URIServerCheckbox.Checked = settings.URIautoStart;
                }

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

            #region Encoder

            //Get Settings Data
            foreach (Libs.Overwrite ow in (Libs.Overwrite[])Enum.GetValues(typeof(Libs.Overwrite)))
                Encoder_OverwriteBox.Items.Add(ow);

            foreach (Libs.VCodec codec in (Libs.VCodec[])Enum.GetValues(typeof(Libs.VCodec)))
                Encoder_VideoCodecBox.Items.Add(codec);

            foreach (Libs.ACodec codec in (Libs.ACodec[])Enum.GetValues(typeof(Libs.ACodec)))
                Encoder_AudioCodecBox.Items.Add(codec);

            foreach (Libs.Preset quality in (Libs.Preset[])Enum.GetValues(typeof(Libs.Preset)))
                Encoder_PresetsBox.Items.Add(quality);

            foreach (Libs.VideoFileExtensions ext in (Libs.VideoFileExtensions[])Enum.GetValues(typeof(Libs.VideoFileExtensions)))
                Encoder_ExtensionBox.Items.Add(ext);

            foreach (Libs.Tune tune in (Libs.Tune[])Enum.GetValues(typeof(Libs.Tune)))
                Encoder_TunerBox.Items.Add(tune);

            foreach (Libs.Size res in (Libs.Size[])Enum.GetValues(typeof(Libs.Size)))
                Encoder_ResolutionBox.Items.Add(res);

            foreach (Libs.Frames fps in (Libs.Frames[])Enum.GetValues(typeof(Libs.Frames)))
                Encoder_FPSBox.Items.Add(fps);

            if (settings.loaded)
            {
                Encoder_OverwriteBox.SelectedIndex = Encoder_OverwriteBox.FindStringExact(settings.overwrite.ToString());
                Encoder_VideoCodecBox.SelectedIndex = Encoder_VideoCodecBox.FindStringExact(settings.vCodec.ToString());
                Encoder_AudioCodecBox.SelectedIndex = Encoder_AudioCodecBox.FindStringExact(settings.aCodec.ToString());
                Encoder_PresetsBox.SelectedIndex = Encoder_PresetsBox.FindStringExact(settings.quality.ToString());
                Encoder_ExtensionBox.SelectedIndex = 0;
                Encoder_TunerBox.SelectedIndex = 0;
                Encoder_ResolutionBox.SelectedIndex = 0;
                Encoder_FPSBox.SelectedIndex = 0;
                Encoder_HideConsoleToggle.Checked = settings.hideConsole;

                if (Directory.Exists(settings.outputLocation))
                    Encoder_OutputFolderTextBox.Text = settings.outputLocation;
                else if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    Encoder_OutputFolderTextBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }
            else //Default settings
            {
                Encoder_OverwriteBox.SelectedIndex = 0;
                Encoder_VideoCodecBox.SelectedIndex = 2;
                Encoder_AudioCodecBox.SelectedIndex = 2;
                Encoder_PresetsBox.SelectedIndex = 0;
                Encoder_ExtensionBox.SelectedIndex = 0;
                Encoder_TunerBox.SelectedIndex = 0;
                Encoder_ResolutionBox.SelectedIndex = 0;
                Encoder_FPSBox.SelectedIndex = 0;
                Encoder_HideConsoleToggle.Checked = false;

                if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    Encoder_OutputFolderTextBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }

            #endregion Encoder


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

        public void SetURIServerStatus(bool status)
        {
            if (status)
                Settings_URIServerIndicator.BackColor = Color.FromArgb(192, 255, 192);
            else
                Settings_URIServerIndicator.BackColor = Color.FromArgb(255, 128, 128);
        }

        private void Settings_DownloadButton_Click(object sender, EventArgs e)
        {
            updater.StartUpdate();
            Core.ChangeTab(Core.Tabs.Updater);
        }

        private void Encoder_OutputFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK) Encoder_OutputFolderTextBox.Text = fb.SelectedPath;
            }
        }

        private void Settings_FFMPEGLocationButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                op.Filter = "Executable Files(*exe.exe)|ffmpeg.exe";

                DialogResult result = op.ShowDialog();
                if (result == DialogResult.OK) Settings_FFmpegPathBox.Text = op.FileName;
            }
        }

        private void Settings_FFPLAYLocationButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                op.Filter = "Executable Files(*exe.exe)|ffplay.exe";

                DialogResult result = op.ShowDialog();
                if (result == DialogResult.OK) Settings_FFplayPathBox.Text = op.FileName;
            }
        }

        private void Settings_DefaultOutputButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK) Settings_DefaultOutputPathBox.Text = fb.SelectedPath;
            }
        }

        private void Encoder_FilesList_DragDrop(object sender, DragEventArgs e)
        {
            //Clear listview.
            Encoder_FilesList.Items.Clear();

            //Get all files dropped by user over the control.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //Detect if there was a directory. Use only directory.
            if (Directory.Exists(files[0])) files = Directory.GetFiles(files[0]);

            //Add data to the listview.
            foreach (string file in files)
            {
                ListViewItem item = new ListViewItem(new[] { file, "• Waiting" });
                Encoder_FilesList.Items.Add(item);
            }
        }

        private void Encoder_FilesList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Encoder_StartEncodingProcessButton_Click(object sender, EventArgs e)
        {
            //Stop if there are no files or application to work with.
            if (!File.Exists(settings.ffmpegPath))
            {
                notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                return;
            }
            if (Encoder_FilesList.Items.Count < 1 || Encoder_FilesList.Items?[0].Text == "Drag and drop a folder or multiple files here...")
            {
                notice.SetNotice("The list of files to encode was empty. Drag files onto the list.", NoticeModule.TypeNotice.Warning);
                return;
            }

            //Create a queue to process.
            ProcesserData processQueueData = new ProcesserData(Encoder_OutputFolderTextBox.Text, Encoder_HideConsoleToggle.Checked);

            //For every file...
            for (int i = 0; i < Encoder_FilesList.Items.Count; i++)
            {
                FileInfo file = new FileInfo(Encoder_FilesList.Items[i].Text);

                // TODO: Add advanced settings for overwrite
                /*
                 *Overwrite
                 *libraries
                 *
                 */

                processQueueData.Add(Libs.Overwrite.Yes, file, (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), Encoder_VideoCodecBox.Text, true), (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), Encoder_AudioCodecBox.Text, true), (Libs.Tune)Enum.Parse(typeof(Libs.Tune), Encoder_TunerBox.Text, true), (Libs.Preset)Enum.Parse(typeof(Libs.Preset), Encoder_PresetsBox.Text, true), (Libs.Frames)Enum.Parse(typeof(Libs.Frames), Encoder_FPSBox.Text, true), (Libs.Size)Enum.Parse(typeof(Libs.Size), Encoder_ResolutionBox.Text, true), (Libs.VideoFileExtensions)Enum.Parse(typeof(Libs.VideoFileExtensions), Encoder_ExtensionBox.Text, true));
            }

            //Start the encoding process.
            encodingProcesser.ProcessFileQueue(processQueueData);
        }

        private void Settings_URIServerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings_URIServerCheckbox.Checked) uriRequestHandler = new UriRequestsHandler(this, Convert.ToInt32(Settings_URIServerPort.Value));
            else uriRequestHandler.KillServer();
        }

        private void HLS_RemoveHLSButton_Click(object sender, EventArgs e)
        {
            if (HLS_listView.SelectedItems.Count > 0) HLS_listView.Items.Remove(HLS_listView.SelectedItems[0]);
        }

        private void HLS_PlayButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(settings.ffplayPath))
            {
                notice.SetNotice("You need to specify a location of \"ffplay.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                return;
            }

            try
            {
                PlayProcess?.Kill();
            }
            catch (Win32Exception) { }
            catch (NotSupportedException) { }
            catch (InvalidOperationException) { }

            //check if there is a selected item in the listview.
            if (HLS_listView.SelectedItems.Count > 0)
            {
                PlayProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = settings.ffplayPath,
                        Arguments = "\"" + HLS_listView.SelectedItems[0].SubItems[1].Text + "\" -autoexit",
                        UseShellExecute = false
                    }
                };

                PlayProcess.Start();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                PlayProcess?.Kill();
            }
            catch (Win32Exception) { }
            catch (NotSupportedException) { }
            catch (InvalidOperationException) { }

            uriRequestHandler.KillServer();
            encodingProcesser.KillThreads();
        }
    }
}
