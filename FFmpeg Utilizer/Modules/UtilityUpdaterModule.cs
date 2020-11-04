#pragma warning disable IDE0044 // Nagging about main

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;

namespace FFmpeg_Utilizer.Modules
{
    public class UtilityUpdaterModule
    {
        private Main main;

        //Client
        private WebClient client = new WebClient();

        //State
        private bool CheckingVersion = false;

        private bool updating = false;
        private bool aborted = false;

        //Temporary version data...
        public string downloadedVersion = "";

        //Calculate speed
        private DateTime now = DateTime.Now;

        private long lastData = 0;
        private Thread extractThread;

        public enum UIState
        {
            Visible,
            Hidden
        }

        public enum UtilityType
        {
            Download,
            Update,
            Stop
        }

        public UtilityUpdaterModule(Main _main) => main = _main;

        public void SetUtilityDownloader(UtilityType type = UtilityType.Download)
        {
            switch (type)
            {
                case UtilityType.Download:
                    main.Update_DownloadButton.Text = "Download Utilities";
                    main.Update_DownloadButton.FlatAppearance.BorderColor = Color.FromArgb(230, 165, 109);
                    main.Settings_DownloadButton.Text = "Download Utilities";
                    main.Settings_DownloadButton.FlatAppearance.BorderColor = Color.FromArgb(230, 165, 109);
                    break;

                case UtilityType.Update:
                    main.Update_DownloadButton.Text = "Update Available";
                    main.Update_DownloadButton.FlatAppearance.BorderColor = Color.FromArgb(32, 191, 107);
                    main.Settings_DownloadButton.Text = "Update Available";
                    main.Settings_DownloadButton.FlatAppearance.BorderColor = Color.FromArgb(32, 191, 107);
                    break;

                case UtilityType.Stop:
                    main.Update_DownloadButton.Text = "Stop Process";
                    main.Update_DownloadButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
                    main.Settings_DownloadButton.Text = "Stop Process";
                    main.Settings_DownloadButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
                    break;
            }
        }

        private void ResetUI()
        {
            main.Update_StatusLabel.Text = "Status";
            main.Update_ProgressBar.Value = 0;
            main.Update_ProgressBar.Maximum = 100;
            main.SpeedLabel.Text = "";
        }

        public void StartUpdateCheckAsync()
        {
            if (updating)
            {
                main.notice.SetNotice("You can not check for updates while already updating.", NoticeModule.TypeNotice.Warning);
                return;
            }
            else if (CheckingVersion)
            {
                main.notice.SetNotice("You are already checking for updates.", NoticeModule.TypeNotice.Warning);
                return;
            }

            CheckingVersion = true;

            client = new WebClient();

            ResetUI();

            main.Update_StatusLabel.Text = "Status: Getting Version...";
            main.Refresh();

            client.DownloadStringCompleted += ClientVersionCheckCompleted;
            client.DownloadStringAsync(new Uri(Core.FFmpeg64BitURLVersion));
        }

        private void ClientVersionCheckCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ResetUI();
            main.Settings_OnlineVerLabel.Text = e.Result;
            main.Update_OnlineVerLabel.Text = e.Result;

            if (e.Result != main.settings.ffVersion && main.settings.ffVersion != "")
                SetUtilityDownloader(UtilityType.Update);
            else
            {
                //If default files was not found and settings are loaded.
                if (main.settings.loaded && !File.Exists(main.settings.ffmpegPath) || !File.Exists(main.settings.ffplayPath))
                {
                    SetUtilityDownloader(UtilityUpdaterModule.UtilityType.Download);
                }
                else if (!main.settings.loaded && !File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg)) || !File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay)))
                {
                    SetUtilityDownloader(UtilityUpdaterModule.UtilityType.Download);
                }
                else
                    SetUtilityDownloader(UtilityUpdaterModule.UtilityType.Download);
            }

            CheckingVersion = false;
        }

        public void StartUpdate()
        {
            if (updating)
            {
                aborted = true;
                client?.CancelAsync();
                extractThread?.Abort();
                updating = false;
                ResetUI();
                SetUtilityDownloader(UtilityType.Download);
                return;
            }
            else if (CheckingVersion)
            {
                main.notice.SetNotice("Check For Update is in progress. Stop process first.", NoticeModule.TypeNotice.Warning);
                return;
            }
            updating = true;
            aborted = false;

            client = new WebClient();

            ResetUI();

            SetUtilityDownloader(UtilityType.Stop);

            main.Update_StatusLabel.Text = "Status: Getting Version...";

            client.DownloadStringCompleted += Wc_DownloadStringCompleted;
            client.DownloadStringAsync(new Uri(Core.FFmpeg64BitURLVersion));
        }

        private void Wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result != "LOCALVERSION")
            {
                downloadedVersion = e.Result;
                main.Update_StatusLabel.Text = "Status: Downloading...";

                client.DownloadProgressChanged += Wc_Download_DownloadProgressChanged;
                client.DownloadFileCompleted += Wc_Download_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(Core.FFmpeg64BitURLDownload + "?" + Core.GetUTCTime()), Core.GetSubfolder(Core.SubFolders.tmp) + "ffmpeg.zip");
            }
            else
            {
                updating = false;
                ResetUI();
            }
        }

        private void Wc_Download_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            main.Update_ProgressBar.Value = e.ProgressPercentage;

            if (DateTime.Now > now.AddSeconds(1))
            {
                main.SpeedLabel.Text = Core.WordNotation(e.BytesReceived - lastData);
                lastData = e.BytesReceived;
                now = DateTime.Now;
            }
        }

        private void Wc_Download_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (aborted)
            {
                ResetUI();
                return;
            }

            main.Update_StatusLabel.Text = "Status: Extracting 0/3";
            main.SpeedLabel.Text = "";
            main.Update_ProgressBar.Value = 0;
            main.Update_ProgressBar.Maximum = 2;
            main.SpeedLabel.Refresh();

            string path = Core.GetSubfolder(Core.SubFolders.Downloads) + "ffmpeg.zip"; // Get path to download folder and file.

            if (File.Exists(path)) File.Delete(path); // If a previous download exist, delete.

            File.Move(Core.GetSubfolder(Core.SubFolders.tmp) + "ffmpeg.zip", path); // Move new file in.

            extractThread = new Thread(() =>
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
                                main.Update_ProgressBar.Value++;
                                main.Update_ProgressBar.Refresh();
                                main.Update_StatusLabel.Text = "Status: Extracting " + main.Update_ProgressBar.Value.ToString() + "/2";
                                main.SpeedLabel.Refresh();
                            }));

                            File.Create(file).Close();
                            entry.ExtractToFile(file, true);
                        }
                    }
                }

                FinishUpdate(path);
            });
            extractThread.Start();
        }

        private void FinishUpdate(string path)
        {
            if (File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg)))
            {
                main.settings.ffmpegPath = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg);

                main.Invoke(new Action(() =>
                {
                    main.Settings_FFmpegPathBox.Text = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffmpeg);
                }));
            }
            if (File.Exists(Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay)))
            {
                main.settings.ffplayPath = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay);

                main.Invoke(new Action(() =>
                {
                    main.Settings_FFplayPathBox.Text = Core.GetSubfolder(Core.SubFolders.Tools) + Core.GetTool(Core.Tools.ffplay);
                }));
            }

            File.Delete(path);

            main.settings.lastUpdate = DateTime.Now;
            main.settings.ffVersion = downloadedVersion;

            main.Invoke(new Action(() =>
            {
                ResetUI();

                main.Update_InstalledVersionLabel.Text = downloadedVersion;
                main.Settings_InstalledVersionLabel.Text = downloadedVersion;
                main.Update_LatestUpdateLabel.Text = main.settings.lastUpdate.ToString();
                main.Settings_LatestUpdateLabel.Text = main.settings.lastUpdate.ToString();

                main.SpeedLabel.Text = "";

                SetUtilityDownloader(UtilityType.Download);
            }));

            main.settings.SaveSettings();

            GC.Collect();

            updating = false;
        }
    }
}