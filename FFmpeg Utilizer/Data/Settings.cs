using Microsoft.Win32;
using System;
using System.Security;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Data
{
    [Serializable]
    public class Settings
    {
        public bool loaded = false;

        private static readonly string registryLocation = @"SOFTWARE\FFmpeg_Utilizer";

        public DateTime? lastUpdate = null;
        public string nextUpdate = "";
        public string ffVersion = "";

        public string ffmpegLoc = "";
        public string ffplayLoc = "";

        public string outputLocation = Core.GetSubfolder(Core.SubFolders.Output);

        public Libs.VCodec vCodec = Libs.VCodec.libx264;
        public Libs.ACodec aCodec = Libs.ACodec.aac;
        public Libs.Preset quality = Libs.Preset.Default;
        public bool hideConsole = false;

        public Settings() => LoadSettings();

        public void ResetSettings()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKey(registryLocation);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentException) { }
            catch (SecurityException) { }
            catch (UnauthorizedAccessException) { }

            lastUpdate = null;
            nextUpdate = "";
            ffVersion = "";
            ffmpegLoc = "";
            ffplayLoc = "";
            hideConsole = false;
            outputLocation = "";
        }

        public void LoadSettings()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(registryLocation);

                if (key != null)
                {
                    object lastUpdateObject = key.GetValue("ffmpeg_lastUpdate");
                    if (lastUpdateObject != null) lastUpdate = DateTime.Parse(lastUpdateObject.ToString());
                    object ffVersionObject = key.GetValue("ffVersion");
                    if (ffVersionObject != null) ffVersion = ffVersionObject.ToString();

                    object ffmpegLocObject = key.GetValue("ffmpeg_location");
                    if (ffmpegLocObject != null) ffmpegLoc = ffmpegLocObject.ToString();
                    object ffplayLocObject = key.GetValue("ffplay_location");
                    if (ffplayLocObject != null) ffplayLoc = ffplayLocObject.ToString();

                    object outputLocationObject = key.GetValue("output_location");
                    if (outputLocationObject != null) outputLocation = outputLocationObject.ToString();

                    object vCodecObject = key.GetValue("vCodec");
                    if (vCodecObject != null) vCodec = (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), vCodecObject.ToString());
                    object aCodecObject = key.GetValue("aCodec");
                    if (aCodecObject != null) aCodec = (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), aCodecObject.ToString());
                    object qualityObject = key.GetValue("quality");
                    if (qualityObject != null) quality = (Libs.Preset)Enum.Parse(typeof(Libs.Preset), qualityObject.ToString());

                    object hideConsoleObject = key.GetValue("hideConsole");
                    if (hideConsoleObject != null) if (hideConsoleObject.ToString() == "1") hideConsole = true;

                    key.Close();

                    loaded = true;
                }
            }
            catch { MessageBox.Show("Unknown error occured when loading settings. Try again..."); }
        }

        public void SaveSettings()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(registryLocation);

                if (key != null)
                {
                    key.SetValue("ffmpeg_lastUpdate", lastUpdate.ToString(), RegistryValueKind.String);
                    key.SetValue("ffVersion", ffVersion, RegistryValueKind.String);

                    key.SetValue("ffmpeg_location", ffmpegLoc, RegistryValueKind.String);
                    key.SetValue("ffplay_location", ffplayLoc, RegistryValueKind.String);

                    key.SetValue("output_location", outputLocation, RegistryValueKind.String);

                    key.SetValue("vCodec", vCodec.ToString(), RegistryValueKind.String);
                    key.SetValue("aCodec", aCodec.ToString(), RegistryValueKind.String);
                    key.SetValue("quality", quality.ToString(), RegistryValueKind.String);

                    if (hideConsole) key.SetValue("hideConsole", "1", RegistryValueKind.String);
                    else key.SetValue("hideConsole", "0", RegistryValueKind.String);

                    key.Close();
                }
            }
            catch (Exception) { MessageBox.Show("Unknown error occured when saving settings. Try again..."); }
        }

        public enum UpdateType
        {
            Download,
            Update
        }

        public UpdateType TimeForUpdate()
        {
            if (lastUpdate == null) return UpdateType.Download;

            if (DateTime.Now >= lastUpdate) return UpdateType.Update;
            else return UpdateType.Download;
        }
    }
}