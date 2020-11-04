using FFmpeg_Utilizer.Data;
using FFmpeg_Utilizer.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FFmpeg_Utilizer
{
    public static class Core
    {
        public static string softwareName = "FFmpeg Utilizer";

        public static string authorRealName = "Victor Rimsby";
        public static string authorName = "N0tiC";
        public static string companyName = "ByteVault Studio";
        public static string authorContact = "contact@bytevaultstudio.se";
        public static string companyWebsite = "http://bytevaultstudio.se/";
        public static string softwareGIT = "Git /n0tic/FFmpeg-UI-Utilizer";
        public static string softwareGITURL = "https://github.com/n0tic/FFmpeg-UI-Utilizer";

        #region Version

        public static BuildTypes buildType = BuildTypes.Alpha;
        public static int majorVersion = 0;
        public static int minorVersion = 1;
        public static int buildVersion = 3;

        public enum BuildTypes
        {
            Alpha,
            Beta,
            Normal
        }

        public static string GetVersion() => majorVersion.ToString() + "." + minorVersion.ToString() + "." + buildVersion.ToString() + " " + buildType.ToString();

        #endregion Version

        #region Files, Folders and Paths

        public static bool IsValidFilename(string testName)
        {
            string strTheseAreInvalidFileNameChars = new string(System.IO.Path.GetInvalidFileNameChars());
            Regex regInvalidFileName = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");

            if (regInvalidFileName.IsMatch(testName)) { return false; };

            return true;
        }

        public static void OpenDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                main.notice.SetNotice("The directory does not exist.", NoticeModule.TypeNotice.Error);
                return;
            }
        }

        public static void CreateAllDefaultFolders()
        {
            try
            {
                foreach (SubFolders folder in Enum.GetValues(typeof(SubFolders)))
                {
                    if (!Directory.Exists(GetSubfolder(folder))) Directory.CreateDirectory(GetSubfolder(folder));
                }
            }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (DirectoryNotFoundException) { }
            catch (InvalidOperationException) { }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            catch (NotSupportedException) { }
        }

        public enum Tools
        {
            ffmpeg,
            ffplay
        }

        public static string GetTool(Tools tool) => tool.ToString() + ".exe";

        public enum SubFolders
        {
            Tools,
            Downloads,
            tmp,
            Output
        }

        public static string GetDirectory() => @AppDomain.CurrentDomain.BaseDirectory;

        public static string GetSubfolder(SubFolders folder) => GetDirectory() + folder.ToString() + @"\";

        public static bool DoesDirectoryExist(SubFolders folder) => Directory.Exists(GetDirectory() + folder.ToString());

        public static bool DoesAllDefaultDirectoryExist()
        {
            if (!Directory.Exists(GetSubfolder(SubFolders.Tools)) ||
                !Directory.Exists(GetSubfolder(SubFolders.tmp)) ||
                !Directory.Exists(GetSubfolder(SubFolders.Downloads)) ||
                !Directory.Exists(GetSubfolder(SubFolders.Output)))
                return false;
            else return true;
        }

        public static bool DoesAllToolsExist()
        {
            if (!File.Exists(GetSubfolder(SubFolders.Tools) + GetTool(Tools.ffmpeg)) ||
                !File.Exists(GetSubfolder(SubFolders.Tools) + GetTool(Tools.ffplay)))
                return false;
            else return true;
        }

        public static bool IsFileLocked(string file)
        {
            FileInfo fileInfo = new FileInfo(file);
            FileStream stream = null;

            try
            {
                stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //File Used
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static void WriteToFile(string data, string file = "tmp.txt")
        {
            try
            {
                using (StreamWriter sw = File.AppendText(GetSubfolder(SubFolders.tmp) + file))
                {
                    sw.WriteLine(data);
                    sw.Close();
                }
            }
            catch (UnauthorizedAccessException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (ArgumentNullException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (ArgumentException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (DirectoryNotFoundException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (PathTooLongException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (NotSupportedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (IOException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
        }

        public static bool WriteToFile(List<string> data, string file = "tmp.txt")
        {
            try
            {
                if (File.Exists(GetSubfolder(SubFolders.tmp) + file)) File.Delete(GetSubfolder(SubFolders.tmp) + file);

                using (StreamWriter sw = File.AppendText(GetSubfolder(SubFolders.tmp) + file))
                {
                    foreach (string f in data)
                    {
                        if (!f.Contains("#"))
                            sw.WriteLine("file '" + f + "'");
                        else
                            sw.WriteLine(f);
                    }
                    sw.Close();
                }
                return true;
            }
            catch (UnauthorizedAccessException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); return false; }
            catch (ArgumentNullException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); return false; }
            catch (ArgumentException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); return false; }
            catch (DirectoryNotFoundException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); return false; }
            catch (PathTooLongException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); return false; }
            catch (NotSupportedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); return false; }
            catch (IOException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); return false; }
        }

        #endregion Files, Folders and Paths

        #region Network

        public static bool IsInternetConnectionAvailable()
        {
            try
            {
                using (var client = new WebClient().OpenRead("http://www.google.com/")) return true;
            }
            catch { return false; }
        }

        public static string GetUTCTime()
        {
            System.Int32 unixTimestamp = (System.Int32)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

        #endregion Network

        #region External FFmpeg Builds

        /*
         * To query the latest available versions, visit the URLs below and parse the single-line plain-text files received:
        https://www.gyan.dev/ffmpeg/builds/git-version
        https://www.gyan.dev/ffmpeg/builds/release-version
        https://www.gyan.dev/ffmpeg/builds/tools-version
        https://www.gyan.dev/ffmpeg/builds/last-build-update
        https://www.gyan.dev/ffmpeg/builds/next-build-update
        https://www.gyan.dev/ffmpeg/builds/changelog-counter

        https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip // Essential release
         */
#if DEBUG
        public static string FFmpeg64BitURLDownload = @"file://C:\Users\N0tiC\Downloads\ffmpeg-4.3.1-2020-10-01-essentials_build.zip";
        public static string FFmpeg64BitURLVersion = @"https://www.gyan.dev/ffmpeg/builds/release-version";
#else
        public static string FFmpeg64BitURLDownload = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";
        public static string FFmpeg64BitURLVersion = "https://www.gyan.dev/ffmpeg/builds/release-version";
#endif

        #endregion External FFmpeg Builds

        #region Word Notation

        private static readonly string[] suffixes = { "Bytes/s", "KB/s", "MB/s", "GB/s", "TB/s", "PB/s" };

        public static string WordNotation(Int64 bytes)
        {
            int counter = 0;
            decimal number = (decimal)bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return string.Format("{0:n1} {1}", number, suffixes[counter]);
        }

        #endregion Word Notation

        #region Application

        #region Move Window

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public static void MoveWindow(Form main, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(main.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion Move Window

        public enum Tabs
        {
            Encoder = 0,
            CutMerge = 1,
            Cut = 3,
            Merge = 2,
            M3U8 = 4,
            Argument = 5,
            Settings = 6,
            Updater = 7
        }

        public enum TabColor
        {
            Selected = 0,
            NotSelected = 1,
            Indicator = 2,
            White = 3,
            NotSectedText = 4
        }

        private static readonly List<Color> colors = new List<Color>() { Color.FromArgb(43, 52, 67), Color.FromArgb(53, 64, 82), Color.FromArgb(25, 124, 237), Color.FromArgb(255, 255, 255), Color.FromArgb(159, 168, 179) };
        public static List<Tab> tabs = new List<Tab>();

        public static void AddTab(Panel tab, Panel indicator, Label text, Panel mainPanel, bool active = true) => tabs.Add(new Tab(tab, indicator, text, mainPanel, active));

        public static Main main;

        public static void ChangeTab(Tabs tab)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                if (i == (int)tab)
                {
                    if (!tabs[i].active) main.notice.SetNotice("Network features disabled. No network connection detected.", Modules.NoticeModule.TypeNotice.Warning);
                    tabs[i].tab.BackColor = colors[(int)TabColor.Selected];
                    tabs[i].indicator.BackColor = colors[(int)TabColor.Indicator];
                    tabs[i].text.ForeColor = colors[(int)TabColor.White];
                    tabs[i].text.BackColor = colors[(int)TabColor.Selected];
                    if (tabs[i].mainPanel != null)
                    {
                        tabs[i].mainPanel.Enabled = true;
                        tabs[i].mainPanel.Visible = true;
                    }
                }
                else
                {
                    tabs[i].tab.BackColor = colors[(int)TabColor.NotSelected];
                    tabs[i].indicator.BackColor = colors[(int)TabColor.NotSelected];
                    tabs[i].text.ForeColor = colors[(int)TabColor.NotSectedText];
                    tabs[i].text.BackColor = colors[(int)TabColor.NotSelected];
                    if (tabs[i].mainPanel != null && (int)tab != 1)
                    {
                        tabs[i].mainPanel.Enabled = false;
                        tabs[i].mainPanel.Visible = false;
                    }
                }

                if ((int)tab == 1 || (int)tab == 2 || (int)tab == 3)
                {
                    tabs[2].tab.Visible = true;
                    tabs[3].tab.Visible = true;
                }
                else
                {
                    tabs[2].tab.Visible = false;
                    tabs[3].tab.Visible = false;
                }
            }
        }

        #endregion Application
    }
}