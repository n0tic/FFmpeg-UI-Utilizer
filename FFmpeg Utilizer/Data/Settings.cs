using FFmpeg_Utilizer.Modules;
using Microsoft.Win32;
using System;
using System.Security;

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

        public string ffmpegPath = "";
        public string ffplayPath = "";
        public string outputLocation = Core.GetSubfolder(Core.SubFolders.Output);

        public Libs.Overwrite overwrite = Libs.Overwrite.Yes;
        public Libs.VCodec vCodec = Libs.VCodec.libx264;
        public Libs.ACodec aCodec = Libs.ACodec.aac;
        public Libs.Preset quality = Libs.Preset.Default;
        public bool hideConsole = false;

        public int URIPort = 288;
        public bool URIautoStart = false;

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

            ffmpegPath = "";
            ffplayPath = "";
            outputLocation = Core.GetSubfolder(Core.SubFolders.Output);

            overwrite = Libs.Overwrite.Ask;
            vCodec = Libs.VCodec.libx264;
            aCodec = Libs.ACodec.aac;
            quality = Libs.Preset.Default;
            hideConsole = false;

            URIPort = 288;
            URIautoStart = false;
        }

        public void LoadSettings()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(registryLocation);

                if (key != null)
                {
                    object lastUpdateObject = key.GetValue("ffmpeg_lastUpdate");
                    if (lastUpdateObject != null && lastUpdateObject.ToString() != "") lastUpdate = DateTime.Parse(lastUpdateObject.ToString());

                    object ffVersionObject = key.GetValue("ffVersion");
                    if (ffVersionObject != null) ffVersion = ffVersionObject.ToString();

                    object ffmpegLocObject = key.GetValue("ffmpeg_location");
                    if (ffmpegLocObject != null) ffmpegPath = ffmpegLocObject.ToString();
                    object ffplayLocObject = key.GetValue("ffplay_location");
                    if (ffplayLocObject != null) ffplayPath = ffplayLocObject.ToString();
                    object outputLocationObject = key.GetValue("output_location");
                    if (outputLocationObject != null) outputLocation = outputLocationObject.ToString();

                    object overwriteObject = key.GetValue("overwrite");
                    if (overwriteObject != null) overwrite = (Libs.Overwrite)Enum.Parse(typeof(Libs.Overwrite), overwriteObject.ToString());
                    object vCodecObject = key.GetValue("vCodec");
                    if (vCodecObject != null) vCodec = (Libs.VCodec)Enum.Parse(typeof(Libs.VCodec), vCodecObject.ToString());
                    object aCodecObject = key.GetValue("aCodec");
                    if (aCodecObject != null) aCodec = (Libs.ACodec)Enum.Parse(typeof(Libs.ACodec), aCodecObject.ToString());
                    object qualityObject = key.GetValue("quality");
                    if (qualityObject != null) quality = (Libs.Preset)Enum.Parse(typeof(Libs.Preset), qualityObject.ToString());
                    object hideConsoleObject = key.GetValue("hideConsole");
                    if (hideConsoleObject != null) if (hideConsoleObject.ToString() == "1") hideConsole = true;

                    object URIPortObject = key.GetValue("URIPort");
                    if (URIPortObject != null) URIPort = Convert.ToInt32(URIPortObject.ToString());
                    object autoStartObject = key.GetValue("autoStart");
                    if (autoStartObject != null) if (autoStartObject.ToString() == "1") URIautoStart = true;

                    key.Close();

                    loaded = true;
                }
            }
            catch (Exception) { }
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

                    key.SetValue("ffmpeg_location", ffmpegPath, RegistryValueKind.String);
                    key.SetValue("ffplay_location", ffplayPath, RegistryValueKind.String);
                    key.SetValue("output_location", outputLocation, RegistryValueKind.String);

                    key.SetValue("overwrite", overwrite.ToString(), RegistryValueKind.String);
                    key.SetValue("vCodec", vCodec.ToString(), RegistryValueKind.String);
                    key.SetValue("aCodec", aCodec.ToString(), RegistryValueKind.String);
                    key.SetValue("quality", quality.ToString(), RegistryValueKind.String);
                    if (hideConsole) key.SetValue("hideConsole", "1", RegistryValueKind.String);
                    else key.SetValue("hideConsole", "0", RegistryValueKind.String);

                    key.SetValue("URIPort", URIPort.ToString(), RegistryValueKind.String);
                    if (URIautoStart) key.SetValue("autoStart", "1", RegistryValueKind.String);
                    else key.SetValue("autoStart", "0", RegistryValueKind.String);

                    key.Close();
                }
            }
            catch (Exception) { }
        }

        public enum UpdateType
        {
            Download,
            Update
        }

        public UtilityUpdaterModule.UtilityType TimeForUpdate(string compareVer)
        {
            if (lastUpdate == null) return UtilityUpdaterModule.UtilityType.Download;

            if (ffVersion != compareVer) return UtilityUpdaterModule.UtilityType.Update;
            else return UtilityUpdaterModule.UtilityType.Download;
        }
    }
}