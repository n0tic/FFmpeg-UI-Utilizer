using System;
using System.IO;

namespace FFmpeg_Utilizer.Data
{
    [Serializable]
    public class EncoderArgument
    {
        public Libs.Overwrite overwrite = Libs.Overwrite.Ask;
        public FileInfo inputFile;
        public Libs.VCodec vCodec = Libs.VCodec.Default;
        public Libs.ACodec aCodec = Libs.ACodec.Default;
        public Libs.Tune tuner = Libs.Tune.Default;
        public Libs.Preset preset = Libs.Preset.Default;
        public Libs.Frames fps = Libs.Frames.Default;
        public Libs.Size res = Libs.Size.Default;
        public string outputFolder;
        public string outputName;
        public Libs.VideoFileExtensions outputExtension;

        public string latestArgOutput = "";

        public EncoderArgument(Libs.Overwrite _overwrite, FileInfo inputFile, Libs.VCodec vC, Libs.ACodec aC, Libs.Tune _tuner, Libs.Preset _preset, Libs.Frames frames, Libs.Size res, string outputDir, string fileName, Libs.VideoFileExtensions fileExt)
        {
            overwrite = _overwrite;
            this.inputFile = inputFile;
            vCodec = vC;
            aCodec = aC;
            tuner = _tuner;
            preset = _preset;
            fps = frames;
            this.res = res;
            outputFolder = outputDir;
            outputName = fileName;
            outputExtension = fileExt;
        }

        public string ExecuteArgs()
        {
            string args = "";

            switch (overwrite)
            {
                case Libs.Overwrite.Yes:
                    args += "-y";
                    break;

                case Libs.Overwrite.No:
                    args += "-n";
                    break;

                default:
                    break;
            }

            if (inputFile != null)
                args += " -i \"" + inputFile.FullName + "\"";
            else
                args += " -i \"C:\\inputfile.avi\"";

            if (vCodec != Libs.VCodec.Default) args += " -c:v " + vCodec.ToString();
            if (aCodec != Libs.ACodec.Default) args += " -c:a " + aCodec.ToString();
            if (tuner != Libs.Tune.Default) args += " -tune " + tuner.ToString();
            if (preset != Libs.Preset.Default) args += " -preset " + preset.ToString();
            if (fps != Libs.Frames.Default) args += " -filter:v fps=fps=" + Libs.GetFramesCount(fps);
            if (res != Libs.Size.Default) args += " -s " + Libs.GetSize(res);

            args += " -f " + outputExtension.ToString();

            args += " \"" + outputFolder + @"\" + outputName + "." + outputExtension + "\"";

            latestArgOutput = args;

            return args;
        }
    }
}