using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFmpeg_Utilizer.Modules
{
    public class UtilityUpdaterModule
    {
        private Main main;

        //Client
        WebClient wc = new WebClient();

        //State
        private bool updating = false;

        //Temporary version data...
        private string downloadedVersion = "";

        //Calculate Download-Speed
        private DateTime now = DateTime.Now;

        private long lastData = 0;

        public enum UIState
        {
            Visible,
            Hidden
        }

        public enum UtilityType
        {
            Download,
            Update
        }

        public UtilityUpdaterModule(Main _main) => main = _main;

        public void SetUtilityDownloader(UtilityType type = UtilityType.Download)
        {
            switch (type)
            {
                case UtilityType.Download:
                    main.Update_DownloadButton.Text = "Download Utilities";
                    main.Update_DownloadButton.FlatAppearance.BorderColor = Color.FromArgb(32, 191, 107);
                    break;
                case UtilityType.Update:
                    main.Update_DownloadButton.Text = "Update Available";
                    break;
            }
        }

        void ResetUI()
        {
            main.Update_StatusLabel.Text = "Status";
            main.InstallProcessBar.Value = 0;
            main.InstallProcessBar.Maximum = 100;
            main.SpeedLabel.Text = "";
        }

        public void StartUpdateCheck()
        {
            if (updating)
            {
                main.notice.SetNotice("You can not check for updates while already updating.", NoticeModule.TypeNotice.Warning);
                return;
            }

            ResetUI();

            main.Update_StatusLabel.Text = "Status: Getting Version...";
            main.Refresh();

            wc.DownloadStringCompleted += wc_VersionCheckCompleted;
            wc.DownloadStringAsync(new Uri(Core.FFmpeg64BitURLVersion));
        }

        private void wc_VersionCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result != "LOCALVERSION")
            {

            }
            else
            {

            }
        }

        public void StartUpdate()
        {
            // TODO: Look into abort downloading. For now we just let the download continue.
            if (updating)
            {
                wc.CancelAsync();
                updating = false;
                ResetUI();
                return;
            }
            updating = true;

            ResetUI();

            main.Update_StatusLabel.Text = "Status: Getting Version...";
            main.Refresh();

            wc.DownloadStringCompleted += Wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri(Core.FFmpeg64BitURLVersion));
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // TODO: Fix Settings
            if (e.Result != "LOCALVERSION")
            {
                main.Update_StatusLabel.Text = "Status: Downloading...";
                main.Refresh();

                wc.DownloadProgressChanged += Wc_Download_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_Download_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(Core.FFmpeg64BitURLDownload + "?" + Core.GetUTCTime()), Core.GetSubfolder(Core.SubFolders.tmp) + "ffmpeg.zip");
            }
            else
            {
                updating = false;
                ResetUI();
            }
        }

        private void Wc_Download_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            main.InstallProcessBar.Value = e.ProgressPercentage;

            if (DateTime.Now > now.AddSeconds(1))
            {
                main.SpeedLabel.Text = Core.WordNotation(e.BytesReceived - lastData);
                lastData = e.BytesReceived;
                now = DateTime.Now;
            }
        }

        private void Wc_Download_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            main.SpeedLabel.Text = "Extracting 0/3";
            main.InstallProcessBar.Value = 0;
            main.InstallProcessBar.Maximum = 3;
            main.SpeedLabel.Refresh();

            string path = Core.GetSubfolder(Core.SubFolders.Downloads) + "ffmpeg.zip"; // Get path to download folder and file.

            if (File.Exists(path)) File.Delete(path); // If a previous download exist, delete.

            File.Move(Core.GetSubfolder(Core.SubFolders.tmp) + "ffmpeg.zip", path); // Move new file in.

            new Thread(() =>
            {
                using (ZipArchive archive = ZipFile.OpenRead(path))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string file = "";

                        if (entry.FullName.Contains(Core.GetTool(Core.Tools.ffmpeg))) file = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg);
                        if (entry.FullName.Contains(Core.GetTool(Core.Tools.ffplay))) file = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay);

                        if (file != "")
                        {
                            main.Invoke(new Action(() =>
                            {
                                main.InstallProcessBar.Value++;
                                main.InstallProcessBar.Refresh();
                                main.SpeedLabel.Text = "Extracting " + main.InstallProcessBar.Value.ToString() + "/3";
                                main.SpeedLabel.Refresh();
                            }));

                            File.Create(file).Close();
                            entry.ExtractToFile(file, true);
                        }
                    }
                }

                FinishUpdate(path);
            }).Start();
        }

        private void FinishUpdate(string path)
        {
            if (File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg)))
            {
                main.settings.ffmpegLoc = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg);

                main.Invoke(new Action(() =>
                {
                    main.Settings_ffmpegLocBox.Text = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg);
                }));
            }
            if (File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay)))
            {
                main.settings.ffplayLoc = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay);

                main.Invoke(new Action(() =>
                {
                    main.Settings_ffplayLocBox.Text = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay);
                }));
            }

            File.Delete(path);

            main.settings.lastUpdate = DateTime.Now;
            main.settings.ffVersion = downloadedVersion;

            main.Invoke(new Action(() =>
            {
                SetUtilityDownloader(UtilityType.Download);

                main.Update_InstalledVersionLabel.Text = downloadedVersion;
                main.Update_LastUpdateLabel.Text = main.settings.lastUpdate.ToString();
                main.Update_LastUpdateLabel.Text = main.settings.lastUpdate.ToString();

                main.InstallProcessLabel.Text = "";
                main.SpeedLabel.Text = "";

                main.InstallProcessButton.Enabled = false;
                main.InstallProcessButton.Visible = false;
                main.Settings_InstallProcessButton.Enabled = true;

                main.tabControl1.SelectedTab = main.tabPage3;
            }));

            main.settings.SaveSettings();

            GC.Collect();

            updating = false;
        }
    }
}
