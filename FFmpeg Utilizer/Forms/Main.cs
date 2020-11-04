#pragma warning disable IDE0044 // Nagging about hasInternet

using FFmpeg_Utilizer.Data;
using FFmpeg_Utilizer.Forms;
using FFmpeg_Utilizer.Modules;
using FFMPEG_Utilizer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

// TODO: Comments
// TODO: ADD libs etc till cut
// TODO: Fix crash on stop update feature...

namespace FFmpeg_Utilizer
{
    public partial class Main : Form
    {
        //Settings
        internal Settings settings = new Settings();

        //Modules
        public NoticeModule notice;

        public UtilityUpdaterModule updater;
        public EncodingProcessor encodingProcessor;
        public M3U8Processor m3u8Processor;
        public CutProcessor cutProcessor;
        public MergeProcessor mergeProcessor;
        public UriRequestsHandler uriRequestHandler;
        public ArgumentsProcesser argumentsProcesser;

        //FFplay process
        private Process PlayProcess;

        // Main's "global" has Internet
        private bool hasInternet = true;

        #region Software Initialization

        public Main()
        {
            InitializeComponent();

            // Initializer for the software.
            SetupSoftware();
        }

        /// <summary>
        /// Set Internet protocols, check connection, start modules and features.
        /// </summary>
        private void SetupSoftware()
        {
            // SecurityProtocol SSL/TSL
            ServicePointManager.Expect100Continue = true; // Not sure if this is needed?
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Check if software can access internet, set "global" bool
            hasInternet = Core.IsInternetConnectionAvailable();

            // Initialize modules with Main references
            SetupModules();

            // Set Internet features on / off
            SetInternetFeatures();
        }

        /// <summary>
        /// References Main and initializes modules.
        /// </summary>
        private void SetupModules()
        {
            //Init Modules
            Core.main = this;
            notice = new NoticeModule(this);
            notice.CloseNotice();
            encodingProcessor = new EncodingProcessor(this);
            m3u8Processor = new M3U8Processor(this);
            cutProcessor = new CutProcessor(this);
            mergeProcessor = new MergeProcessor(this);
            argumentsProcesser = new ArgumentsProcesser(this);

            // If we have Internet, initialize updater.
            if (hasInternet)
                updater = new UtilityUpdaterModule(this);
        }

        /// <summary>
        /// If no internet connection was detected, disable network features.
        /// </summary>
        private void SetInternetFeatures()
        {
            //If we dont have internet, disable.
            if (!hasInternet)
            {
                hasInternet = false;
                M3U8MainPanel.Dispose();
                Settings_NetPanel.Dispose();
                UpdateMainPanel.Dispose();
                notice.SetNotice("Network features disabled. No network connection detected.", NoticeModule.TypeNotice.Info);
            }
        }

        /// <summary>
        /// Main Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            notice.SetNotice("This software works best with the 'Libx264' Video Codec and 'AAC' Audio Codec library.", NoticeModule.TypeNotice.Info);

            // If we have determined we have internet; Check for updates.
            if (hasInternet)
                updater.StartUpdateCheckAsync();

            //Check if default folders exist, or create them.
            SetupFolders();

            //Setup UI Elements.
            SetupUI();
        }

        /// <summary>
        /// This will check if all default folders exist. If they dont, it creates them.
        /// </summary>
        private void SetupFolders()
        {
            //Check if default directory exist.
            if (!Core.DoesAllDefaultDirectoryExist())
                Core.CreateAllDefaultFolders(); // Create folders.
        }

        /// <summary>
        /// Setup by loading and settings default values to UI elements.
        /// </summary>
        private void SetupUI()
        {
            // Add all tabs and references.
            AddTabs();
            // Set first tab active.
            Core.ChangeTab(0);

            // If we are in debug, show the lower buttons to control the notice feature.
#if DEBUG
            Button1.Visible = true;
            Button2.Visible = true;
            Button3.Visible = true;
            button4.Visible = true;
#endif

            #region Settings

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
                else if (File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg)))
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
                if (settings.URIautoStart)
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

            #region M3U8

            if (settings.loaded)
            {
                M3U8_HideConsoleCheckbox.Checked = settings.hideConsole;

                if (Directory.Exists(settings.outputLocation))
                    M3U8_OutputFolderTextbox.Text = settings.outputLocation;
                else if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    M3U8_OutputFolderTextbox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }
            else
            {
                if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    M3U8_OutputFolderTextbox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }

            #endregion M3U8

            #region Cut

            foreach (Libs.VCodec codec in (Libs.VCodec[])Enum.GetValues(typeof(Libs.VCodec)))
                Cut_VideoCodecBox.Items.Add(codec);

            foreach (Libs.ACodec codec in (Libs.ACodec[])Enum.GetValues(typeof(Libs.ACodec)))
                Cut_AudioCodecBox.Items.Add(codec);

            foreach (Libs.Preset quality in (Libs.Preset[])Enum.GetValues(typeof(Libs.Preset)))
                Cut_PresetBox.Items.Add(quality);
            Cut_PresetBox.SelectedIndex = 0;

            for (int i = 0; i <= 51; i++) Cut_CRFBox.Items.Add(i.ToString());

            if (settings.loaded)
            {
                Cut_HideConsoleToggle.Checked = settings.hideConsole;
                Cut_VideoCodecBox.SelectedIndex = Cut_VideoCodecBox.FindStringExact(settings.vCodec.ToString());
                Cut_AudioCodecBox.SelectedIndex = Cut_AudioCodecBox.FindStringExact(settings.aCodec.ToString());

                Cut_CRFBox.SelectedIndex = 0;

                if (Directory.Exists(settings.outputLocation))
                    Cut_OutputDirectoryBox.Text = settings.outputLocation;
                else if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    Cut_OutputDirectoryBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }
            else
            {
                Cut_VideoCodecBox.SelectedIndex = 3;
                Cut_AudioCodecBox.SelectedIndex = 2;

                Cut_CRFBox.SelectedIndex = 0;

                if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    Cut_OutputDirectoryBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }

            #endregion Cut

            #region Merge

            if (settings.loaded)
            {
                Merge_HideConsoleToggle.Checked = settings.hideConsole;

                if (Directory.Exists(settings.outputLocation))
                    Merge_OutputDirectoryTextbox.Text = settings.outputLocation;
                else if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    Merge_OutputDirectoryTextbox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }
            else
            {
                if (Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output)))
                    Merge_OutputDirectoryTextbox.Text = Core.GetSubfolder(Core.SubFolders.Output);
            }

            #endregion Merge

            //Set lower left information
            SoftwareLabel.Text = Core.softwareName + " " + Core.GetVersion();
            AuthorLabel.Text = Core.authorRealName + " AKA " + Core.authorName;
            GitLabel.Text = Core.softwareGIT;
        }

        #region Tabs

        /// <summary>
        /// Add all tabs, in order, to be referenced.
        /// </summary>
        private void AddTabs()
        {
            //Set reference;
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

        #endregion Software Initialization

        #region Software Window

        /// <summary>
        /// Show information form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoIcon_Click(object sender, EventArgs e) => new SoftwareInformation().ShowDialog();

        /// <summary>
        /// Feature to show software when put in tray.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TraySystem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            TraySystem.Visible = false;
        }

        /// <summary>
        /// Send software to tray.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToTrayButton_Click(object sender, EventArgs e)
        {
            Hide();
            TraySystem.Visible = true;

            TraySystem.ShowBalloonTip(1000);
        }

        /// <summary>
        /// Send user to GIT repo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GitLabel_Click(object sender, EventArgs e) => Process.Start(Core.softwareGITURL);

        /// <summary>
        /// Close notice message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoticeCloseButton_Click(object sender, EventArgs e) => notice.CloseNotice();

        /// <summary>
        /// Close/Quit software.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoftwareCloseButton_Click(object sender, EventArgs e) => Application.Exit();

        /// <summary>
        /// Minimize the software.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SoftwareMinimizeButton_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        /// <summary>
        ///Make software movable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopLogo_MouseDown(object sender, MouseEventArgs e) => Core.MoveWindow(this, e);

        /// <summary>
        /// Runs when the software is about to close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                PlayProcess?.Kill();
            }
            catch (Win32Exception) { }
            catch (NotSupportedException) { }
            catch (InvalidOperationException) { }

            //Kill all modules and their respective threads.
            encodingProcessor?.KillThreads();
            m3u8Processor?.KillThreads();
            cutProcessor?.KillThreads();
            mergeProcessor?.KillThreads();
            uriRequestHandler?.KillServer();
            argumentsProcesser?.KillThreads();
        }

        #endregion Software Window

        #region Testing

        // Buttons and text to show when pressing test buttons.
        private string _txt = "A notice system for errors, warnings, infos and successful operations.";

        private void Button1_Click(object sender, EventArgs e) => notice.SetNotice(_txt, NoticeModule.TypeNotice.Warning);

        private void Button2_Click(object sender, EventArgs e) => notice.SetNotice(_txt, NoticeModule.TypeNotice.Info);

        private void Button3_Click(object sender, EventArgs e) => notice.SetNotice(_txt, NoticeModule.TypeNotice.Error);

        private void Button4_Click(object sender, EventArgs e) => notice.SetNotice(_txt, NoticeModule.TypeNotice.Success);

        #endregion Testing

        #region Update Feature

        /// <summary>
        /// Check for updates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_CheckForUpdateButton_Click(object sender, EventArgs e) => updater.StartUpdateCheckAsync();

        /// <summary>
        /// Updates ffmpeg.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update_DownloadButton_Click(object sender, EventArgs e) => updater.StartUpdate();

        #endregion Update Feature

        #region Settings

        /// <summary>
        /// Open directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_OpenDirectoryButton_Click(object sender, EventArgs e) => Core.OpenDirectory(Settings_DefaultOutputPathBox.Text);

        /// <summary>
        /// Sets the default output folder to the default subfolder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_AutoDefaultOutputButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output))) Directory.CreateDirectory(Core.GetSubfolder(Core.SubFolders.Output));

            settings.outputLocation = Core.GetSubfolder(Core.SubFolders.Output);
            Settings_DefaultOutputPathBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
        }

        /// <summary>
        /// Sets data and saves settings to registry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Resets settings and saves.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Change indicator status.
        /// </summary>
        /// <param name="status"></param>
        public void SetURIServerStatus(bool status)
        {
            if (status)
                Settings_URIServerIndicator.BackColor = Color.FromArgb(192, 255, 192);
            else
                Settings_URIServerIndicator.BackColor = Color.FromArgb(255, 128, 128);
        }

        /// <summary>
        /// Download Update and switch tab to Update utility.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_DownloadButton_Click(object sender, EventArgs e)
        {
            updater.StartUpdate();
            Core.ChangeTab(Core.Tabs.Updater);
        }

        /// <summary>
        /// Set FFmpeg path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_FFMPEGLocationButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                op.Filter = "Executable Files(*exe.exe)|ffmpeg.exe";

                DialogResult result = op.ShowDialog();
                if (result == DialogResult.OK) Settings_FFmpegPathBox.Text = op.FileName;
            }
        }

        /// <summary>
        /// Set FFplay path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_FFPLAYLocationButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                op.Filter = "Executable Files(*exe.exe)|ffplay.exe";

                DialogResult result = op.ShowDialog();
                if (result == DialogResult.OK) Settings_FFplayPathBox.Text = op.FileName;
            }
        }

        /// <summary>
        /// Set default output path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_DefaultOutputButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK) Settings_DefaultOutputPathBox.Text = fb.SelectedPath;
            }
        }

        /// <summary>
        /// Start/Kill uriRequester feature.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_URIServerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (Settings_URIServerCheckbox.Checked) uriRequestHandler = new UriRequestsHandler(this, Convert.ToInt32(Settings_URIServerPort.Value));
            else uriRequestHandler.KillServer();
        }

        #endregion Settings

        #region Encoder

        /// <summary>
        /// Open directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encoder_OpenDirectoryButton_Click(object sender, EventArgs e) => Core.OpenDirectory(Encoder_OutputFolderTextBox.Text);

        /// <summary>
        /// Set default output directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encoder_DefaultOutputButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output))) Directory.CreateDirectory(Core.GetSubfolder(Core.SubFolders.Output));
            Encoder_OutputFolderTextBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
        }

        /// <summary>
        /// Play selected media.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encoder_PlayButton_Click(object sender, EventArgs e)
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
            if (Encoder_FilesList.SelectedItems.Count > 0)
            {
                PlayProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = settings.ffplayPath,
                        Arguments = "\"" + Encoder_FilesList.SelectedItems[0].Text + "\" -autoexit",
                        UseShellExecute = false
                    }
                };

                PlayProcess.Start();
            }
        }

        /// <summary>
        /// Set encoder output folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encoder_OutputFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK) Encoder_OutputFolderTextBox.Text = fb.SelectedPath;
            }
        }

        /// <summary>
        /// Handle dragged files onto listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encoder_FilesList_DragDrop(object sender, DragEventArgs e)
        {
            if (encodingProcessor.encodingInProcess)
            {
                notice.SetNotice("Encoder is already processing. You cannot add or remove elements of the list while the process is active.", NoticeModule.TypeNotice.Error);
                return;
            }

            List<string> allowedExtensions = Libs.GetAllowedExtension();

            //Clear listview.
            Encoder_FilesList.Items.Clear();

            //Get all files dropped by user over the control.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //Detect if there was a directory. Use only directory.
            if (Directory.Exists(files[0])) files = Directory.GetFiles(files[0]);

            //Add data to the listview.
            foreach (string file in files)
            {
                FileInfo f = new FileInfo(file);
                foreach (string ext in allowedExtensions)
                {
                    if ("." + ext == f.Extension)
                    {
                        ListViewItem item = new ListViewItem(new[] { file, "• Waiting" });
                        Encoder_FilesList.Items.Add(item);
                        break;
                    }
                }
            }

            if (files.Length > Encoder_FilesList.Items.Count)
            {
                notice.SetNotice("Some files has been filtered out since they were not an allowed extension.", NoticeModule.TypeNotice.Warning);
            }
        }

        /// <summary>
        /// Change visually that drop is accepted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Encoder_FilesList_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Start encoding process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                notice.SetNotice("The list of files to encode was empty. Drag files onto the list.", NoticeModule.TypeNotice.Error);
                return;
            }

            //Create a queue to process.
            EncodeProcesserData processQueueData = new EncodeProcesserData(Encoder_OutputFolderTextBox.Text, Encoder_HideConsoleToggle.Checked);

            //For every file...
            for (int i = 0; i < Encoder_FilesList.Items.Count; i++)
            {
                FileInfo file = new FileInfo(Encoder_FilesList.Items[i].Text);
                processQueueData.Add((Libs.Overwrite)Enum.Parse(typeof(Libs.Overwrite), Encoder_OverwriteBox.Text, true), file, (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), Encoder_VideoCodecBox.Text, true), (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), Encoder_AudioCodecBox.Text, true), (Libs.Tune)Enum.Parse(typeof(Libs.Tune), Encoder_TunerBox.Text, true), (Libs.Preset)Enum.Parse(typeof(Libs.Preset), Encoder_PresetsBox.Text, true), (Libs.Frames)Enum.Parse(typeof(Libs.Frames), Encoder_FPSBox.Text, true), (Libs.Size)Enum.Parse(typeof(Libs.Size), Encoder_ResolutionBox.Text, true), (Libs.VideoFileExtensions)Enum.Parse(typeof(Libs.VideoFileExtensions), Encoder_ExtensionBox.Text, true));
            }

            //Start the encoding process.
            encodingProcessor.ProcessFileQueue(processQueueData);
        }

        #endregion Encoder

        #region M3U8

        /// <summary>
        /// Open directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M3U8_OpenDirectoryButton_Click(object sender, EventArgs e) => Core.OpenDirectory(M3U8_OutputFolderTextbox.Text);

        /// <summary>
        /// Set default output folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M3U8_DefaultOutputButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output))) Directory.CreateDirectory(Core.GetSubfolder(Core.SubFolders.Output));
            M3U8_OutputFolderTextbox.Text = Core.GetSubfolder(Core.SubFolders.Output);
        }

        /// <summary>
        /// Remove M3U8 list item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HLS_RemoveHLSButton_Click(object sender, EventArgs e)
        {
            if (m3u8Processor.inProcess)
            {
                notice.SetNotice("M3U8 is already processing. You cannot add or remove elements of the list while the process is active.", NoticeModule.TypeNotice.Error);
                return;
            }

            if (M3U8_listView.SelectedItems.Count > 0) M3U8_listView.Items.Remove(M3U8_listView.SelectedItems[0]);
        }

        /// <summary>
        /// Preview M3U8 stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            if (M3U8_listView.SelectedItems.Count > 0)
            {
                PlayProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = settings.ffplayPath,
                        Arguments = "\"" + M3U8_listView.SelectedItems[0].SubItems[1].Text + "\" -autoexit",
                        UseShellExecute = false
                    }
                };

                PlayProcess.Start();
            }
        }

        /// <summary>
        /// Set Default output folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M3U8_OutputButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                DialogResult result = fb.ShowDialog();
                if (result == DialogResult.OK) M3U8_OutputFolderTextbox.Text = fb.SelectedPath;
            }
        }

        /// <summary>
        /// Show and add M3U8 form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M3U8_AddM3U8Button_Click(object sender, EventArgs e)
        {
            if (m3u8Processor.inProcess)
            {
                notice.SetNotice("M3U8 is already processing. You cannot add or remove elements of the list while the process is active.", NoticeModule.TypeNotice.Error);
                return;
            }

            using (AddM3U8URL form = new AddM3U8URL())
            {
                DialogResult res = form.ShowDialog();
                if (res == DialogResult.OK)
                {
                    AddM3U8URLData(form.NameField.Text, form.URLField.Text);
                }
            }
        }

        /// <summary>
        /// Parse and add M3U8 item.
        /// </summary>
        /// <param name="customName"></param>
        /// <param name="url"></param>
        private void AddM3U8URLData(string customName, string url)
        {
            string name;
            if (customName == "" || customName == "Name...") name = Path.GetFileNameWithoutExtension(new Uri(url).LocalPath);
            else name = customName;

            ListViewItem item = new ListViewItem(new[] { name, url, "• Waiting" });
            M3U8_listView.Items.Add(item);
        }

        /// <summary>
        /// Start M3U8 download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M3U8_StartButton_Click(object sender, EventArgs e)
        {
            //Stop if there are no files or application to work with.
            if (!File.Exists(settings.ffmpegPath))
            {
                notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                return;
            }
            if (M3U8_listView.Items.Count < 1)
            {
                notice.SetNotice("The list of files to encode was empty. Aborting.", NoticeModule.TypeNotice.Error);
                return;
            }

            //Create a queue to process.
            M3U8ProcesserData processQueueData = new M3U8ProcesserData(M3U8_OutputFolderTextbox.Text, M3U8_HideConsoleCheckbox.Checked);

            //For every file...
            for (int i = 0; i < M3U8_listView.Items.Count; i++)
                processQueueData.Add(M3U8_listView.Items[i].SubItems[0].Text, M3U8_listView.Items[i].SubItems[1].Text);

            //Start the encoding process.
            m3u8Processor.ProcessFileQueue(processQueueData);
        }

        #endregion M3U8

        #region Cut

        /// <summary>
        /// Open directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_OpenDirectoryButton_Click(object sender, EventArgs e) => Core.OpenDirectory(Cut_OutputDirectoryBox.Text);

        /// <summary>
        /// Set Default output folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_DefaultOutputButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output))) Directory.CreateDirectory(Core.GetSubfolder(Core.SubFolders.Output));
            Cut_OutputDirectoryBox.Text = Core.GetSubfolder(Core.SubFolders.Output);
        }

        /// <summary>
        /// Set input media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_InputMediaButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog op = new OpenFileDialog())
            {
                // TODO: Add real filters to everything?
                //op.Filter = "Executable Files(*exe.exe)|ffmpeg.exe";

                DialogResult result = op.ShowDialog();
                if (result == DialogResult.OK) Cut_MediaInputTextbox.Text = op.FileName;
            }
        }

        /// <summary>
        /// Convert input to strings
        /// </summary>
        /// <returns></returns>
        private string GetTimespanString()
        {
            Decimal startHour = Cut_StartHours.Value;
            Decimal startMinute = Cut_StartMinutes.Value;
            Decimal startSeconds = Cut_StartSeconds.Value;
            Decimal startMiliseconds = Cut_StartMiliseconds.Value;

            Decimal endHour = Cut_EndHours.Value;
            Decimal endMinute = Cut_EndMinutes.Value;
            Decimal endSeconds = Cut_EndSeconds.Value;
            Decimal endMiliseconds = Cut_EndMiliseconds.Value;

            string startMinutes;
            if (startMinute < 10) startMinutes = "0" + startMinute.ToString();
            else startMinutes = startMinute.ToString();

            string endMinutes;
            if (endMinute < 10) endMinutes = "0" + endMinute.ToString();
            else endMinutes = endMinute.ToString();

            string start = startHour.ToString() + ":" + startMinutes + ":" + startSeconds.ToString() + "." + startMiliseconds.ToString();
            string end = endHour.ToString() + ":" + endMinutes + ":" + endSeconds.ToString() + "." + endMiliseconds.ToString();

            decimal _end = endHour + endMinute + endSeconds + endMiliseconds;

            if (_end == 0) Cut_PreviewLabel.ForeColor = Color.FromArgb(255, 128, 128);
            else Cut_PreviewLabel.ForeColor = Color.FromArgb(0, 0, 0);

            return start + " - " + end;
        }

        /// <summary>
        /// Add timespans if they don't already exist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_AddTimespanButton_Click(object sender, EventArgs e)
        {
            if (cutProcessor.inProcess)
            {
                notice.SetNotice("Cutting is already processing. You cannot add or remove elements of the list while the process is active.", NoticeModule.TypeNotice.Error);
                return;
            }

            string timespan = GetTimespanString();
            string end = timespan.Split(' ')[2];

            if (end != "0:00:0.0" && Cut_listView.FindItemWithText(timespan) == null)
            {
                ListViewItem item = new ListViewItem(timespan);
                Cut_listView.Items.Add(item);
            }
        }

        /// <summary>
        /// On value Change on Cut timers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_StartHours_ValueChanged(object sender, EventArgs e) => Cut_PreviewLabel.Text = GetTimespanString();

        /// <summary>
        /// Remove timespan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_RemoveSelectedButton_Click(object sender, EventArgs e)
        {
            if (cutProcessor.inProcess)
            {
                notice.SetNotice("Cutting is already processing. You cannot add or remove elements of the list while the process is active.", NoticeModule.TypeNotice.Error);
                return;
            }

            if (Cut_listView.SelectedItems.Count > 0) Cut_listView.Items.Remove(Cut_listView.SelectedItems[0]);
        }

        /// <summary>
        /// Start cutting media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_StartCuttingButton_Click(object sender, EventArgs e)
        {
            //Stop if there are no files or application to work with.
            if (!File.Exists(settings.ffmpegPath))
            {
                notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                return;
            }
            if (!File.Exists(Cut_MediaInputTextbox.Text))
            {
                notice.SetNotice("You have selected an input media that appears to not exist. Try again.", NoticeModule.TypeNotice.Error);
                return;
            }
            if (Cut_listView.Items.Count < 1)
            {
                notice.SetNotice("The list of timestamps to cut/encode was empty. Add timestamp(s).", NoticeModule.TypeNotice.Warning);
                return;
            }

            //Create a queue to process.
            Queue<TimeStamps> queue = new Queue<TimeStamps>();

            //For every file...
            for (int i = 0; i < Cut_listView.Items.Count; i++)
            {
                string start, end;
                string[] split = Cut_listView.Items[i].Text.Split(' ');
                start = split[0];
                end = split[2];

                queue.Enqueue(new TimeStamps(start, end, queue.Count + 1));
            }

            CutArgument processQueueData = new CutArgument(new FileInfo(Cut_MediaInputTextbox.Text), Cut_OutputDirectoryBox.Text, queue, (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), Cut_VideoCodecBox.Text, true), (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), Cut_AudioCodecBox.Text, true), Cut_CRFBox.Text, (Libs.Preset)Enum.Parse(typeof(Libs.Preset), Cut_PresetBox.Text, true));

            //Start the encoding process.
            cutProcessor.ProcessFileQueue(processQueueData);
        }

        #endregion Cut

        #region Merge

        /// <summary>
        /// Open directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Merge_OpenDirectoryButton_Click(object sender, EventArgs e) => Core.OpenDirectory(Merge_OutputDirectoryTextbox.Text);

        /// <summary>
        /// Set default output folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Merge_DefaultOutputButton_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Core.GetSubfolder(Core.SubFolders.Output))) Directory.CreateDirectory(Core.GetSubfolder(Core.SubFolders.Output));
            Merge_OutputDirectoryTextbox.Text = Core.GetSubfolder(Core.SubFolders.Output);
        }

        /// <summary>
        /// Handle dragged files onto listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Merge_listView_DragDrop(object sender, DragEventArgs e)
        {
            if (mergeProcessor.inProcess)
            {
                notice.SetNotice("Merge is already processing. You cannot add or remove elements of the list while the process is active.", NoticeModule.TypeNotice.Error);
                return;
            }

            List<string> allowedExtensions = Libs.GetAllowedExtension();

            //Clear listview.
            Merge_listView.Items.Clear();

            //Get all files dropped by user over the control.
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //Detect if there was a directory. Use only directory.
            if (Directory.Exists(files[0])) files = Directory.GetFiles(files[0]);

            //Add data to the listview.
            foreach (string file in files)
            {
                FileInfo f = new FileInfo(file);
                foreach (string ext in allowedExtensions)
                {
                    if ("." + ext == f.Extension)
                    {
                        ListViewItem item = new ListViewItem(new[] { (Merge_listView.Items.Count + 1).ToString(), file });
                        Merge_listView.Items.Add(item);
                        break;
                    }
                }
            }

            if (files.Length > Merge_listView.Items.Count)
            {
                notice.SetNotice("Some files has been filtered out since they were not an allowed extension.", NoticeModule.TypeNotice.Warning);
            }
        }

        /// <summary>
        /// Change visually that drop is accepted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Merge_listView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Start merging files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Merge_StartButton_Click(object sender, EventArgs e)
        {
            if (Merge_listView.Items[0].SubItems[1].Text == "Drag and drop a folder or multiple files here...")
            {
                notice.SetNotice("File list can not be empty. Drag files to merge onto the list.", NoticeModule.TypeNotice.Error);
                return;
            }

            List<string> data = new List<string>() { "# This is the list, in order, from which the new file will generated from." };
            foreach (ListViewItem item in Merge_listView.Items)
                data.Add(item.SubItems[1].Text);

            if (Core.WriteToFile(data, "tmptest.txt") && Directory.Exists(Merge_OutputDirectoryTextbox.Text) && Core.IsValidFilename(Merge_OutputFileName.Text))
            {
                MergeProcesserData processData = new MergeProcesserData(Core.GetSubfolder(Core.SubFolders.tmp) + "tmptest.txt", Merge_OutputDirectoryTextbox.Text, Merge_OutputFileName.Text, Path.GetExtension(Merge_listView.Items[0].SubItems[1].Text), Merge_HideConsoleToggle.Checked);
                mergeProcessor.ProcessMerge(processData);
            }
        }

        /// <summary>
        /// Show data from selected media.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Merge_listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Merge_listView.SelectedItems.Count > 0 && Merge_listView.SelectedItems[0].SubItems[1].Text != "Drag & drop here...")
            {
                FileInfo file = new FileInfo(Merge_listView.SelectedItems[0].SubItems[1].Text);
                Merge_mediaOrderLabel.Text = Merge_listView.SelectedItems[0].SubItems[0].Text;
                Merge_mediaPathLabel.Text = file.FullName;
                toolTip.SetToolTip(Merge_mediaPathLabel, file.FullName);
                if (file.Exists)
                    Merge_SizeLabel.Text = Core.WordNotation(file.Length).Replace("/s", "");
                else
                    Merge_SizeLabel.Text = "File moved or deleted.";
                Merge_mediaExtensionLabel.Text = file.Extension;
                List<string> knownExtensions = CommonExtensions.GetAllExtensionsList();
                int known = -1;
                for (int i = 0; i < knownExtensions.Count; i++)
                {
                    if (knownExtensions[i] == file.Extension)
                    {
                        known = i;
                        break;
                    }
                }
                if (known > -1)
                {
                    List<string> desc = CommonExtensions.GetAllExtensionsDescriptionsList();
                    Merge_mediaExtensionDescLabel.Text = desc[known];
                }
            }
            else
            {
                Merge_mediaOrderLabel.Text = "";
                Merge_mediaPathLabel.Text = "";
                toolTip.SetToolTip(Merge_mediaPathLabel, "");
                Merge_SizeLabel.Text = "";
                Merge_mediaExtensionLabel.Text = "";
                Merge_mediaExtensionDescLabel.Text = "";
            }
        }

        #endregion Merge

        #region Argument

        /// <summary>
        /// Builds the encode string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Argument_ShowEncodeButton_Click(object sender, EventArgs e)
        {
            EncoderArgument args = new EncoderArgument((Libs.Overwrite)Enum.Parse(typeof(Libs.Overwrite), Encoder_OverwriteBox.Text, true), null, (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), Encoder_VideoCodecBox.Text, true), (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), Encoder_AudioCodecBox.Text, true), (Libs.Tune)Enum.Parse(typeof(Libs.Tune), Encoder_TunerBox.Text, true), (Libs.Preset)Enum.Parse(typeof(Libs.Preset), Encoder_PresetsBox.Text, true), (Libs.Frames)Enum.Parse(typeof(Libs.Frames), Encoder_FPSBox.Text, true), (Libs.Size)Enum.Parse(typeof(Libs.Size), Encoder_ResolutionBox.Text, true), Encoder_OutputFolderTextBox.Text, "filename", (Libs.VideoFileExtensions)Enum.Parse(typeof(Libs.VideoFileExtensions), Encoder_ExtensionBox.Text, true));
            Argument_PreviewBox.Text = args.ExecuteArgs();
        }

        /// <summary>
        /// Builds the cut string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Argument_ShowCutButton_Click(object sender, EventArgs e)
        {
            Queue<TimeStamps> queue = new Queue<TimeStamps>();
            queue.Enqueue(new TimeStamps("00:00:00.000", "00:00:30.000", 0));

            FileInfo file;
            if (File.Exists(Cut_MediaInputTextbox.Text)) file = new FileInfo(Cut_MediaInputTextbox.Text);
            else file = null;

            CutArgument args = new CutArgument(file, Cut_OutputDirectoryBox.Text, queue, (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), Cut_VideoCodecBox.Text, true), (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), Cut_AudioCodecBox.Text, true), Cut_CRFBox.Text, (Libs.Preset)Enum.Parse(typeof(Libs.Preset), Cut_PresetBox.Text, true));

            Argument_PreviewBox.Text = args.ExecuteArgs();
        }

        /// <summary>
        /// Builds the M3U8 string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Argument_ShowM3U8Button_Click(object sender, EventArgs e) => Argument_PreviewBox.Text = "-y -i \"http://whateverurl.com/stuff.m3u8\" -acodec copy -vcodec copy -absf aac_adtstoasc \"c:\\outputfolder\\outputfile.mp4\"";

        /// <summary>
        /// Builds the Merge string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Argument_ShowMergeButton_Click(object sender, EventArgs e) => Argument_PreviewBox.Text = "-y -f concat -safe 0 -i \"C:\\filelist.txt\" \"C:\\outputfolder\\OutputFileName.extension\"";

        /// <summary>
        /// Run custom argument
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Argument_RunArgumentButton_Click(object sender, EventArgs e) => argumentsProcesser.ProcessArgs(Argument_PreviewBox.Text);

        /// <summary>
        /// Clear argument box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Argument_ClearButton_Click(object sender, EventArgs e) => Argument_PreviewBox.Text = "";

        #endregion Argument
    }
}