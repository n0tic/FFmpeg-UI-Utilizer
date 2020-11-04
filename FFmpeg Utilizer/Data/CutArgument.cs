using System.Collections.Generic;
using System.IO;

namespace FFmpeg_Utilizer.Data
{
    public class CutArgument
    {
        public string outputFolder;
        public FileInfo inputMedia;
        public Queue<TimeStamps> timestamps;
        public Libs.VCodec vCodec;
        public Libs.ACodec aCodec;
        public string crf;
        public Libs.Preset preset;

        public CutArgument(FileInfo inputMedia, string outputFolder, Queue<TimeStamps> timestamps, Libs.VCodec vCodec, Libs.ACodec aCodec, string crf, Libs.Preset preset)
        {
            this.inputMedia = inputMedia;
            this.outputFolder = outputFolder;
            this.timestamps = timestamps;
            this.vCodec = vCodec;
            this.aCodec = aCodec;
            this.crf = crf;
            this.preset = preset;
        }

        public string ExecuteArgs()
        {
            string args;

            args = "-y -i ";

            if (inputMedia != null)
                args += "\"" + inputMedia.FullName + "\"";
            else args += "\"C:\\inputfile.mp4\"";

            if (vCodec != Libs.VCodec.Default) args += " -c:v " + vCodec.ToString();
            if (aCodec != Libs.ACodec.Default) args += " -c:a " + aCodec.ToString();
            args += " -crf " + crf;
            if (preset != Libs.Preset.Default) args += " -preset " + preset.ToString();
            args += " -ss " + timestamps.Peek().startTime;
            args += " -t " + timestamps.Peek().endTime;
            args += " -map 0";

            if (inputMedia != null)
                args += " \"" + outputFolder + "\\" + Path.GetFileNameWithoutExtension(inputMedia.Name) + "_" + timestamps.Peek().id.ToString() + inputMedia.Extension + "\"";
            else args += " \"C:\\outputfolder\\outputfile_num.ext\"";

            return args;
        }
    }

    public class TimeStamps
    {
        public int id;
        public string startTime;
        public string endTime;

        public TimeStamps(string startTime, string endTime, int id)
        {
            this.id = id;
            this.startTime = startTime;
            this.endTime = endTime;
        }
    }
}