using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpeg_Utilizer.Data
{
    public class CutArgument
    {
        public string outputFolder;
        public FileInfo inputMedia;
        public Queue<TimeStamps> timestamps;

        public CutArgument(FileInfo inputMedia, string outputFolder, Queue<TimeStamps> timestamps)
        {
            this.inputMedia = inputMedia;
            this.outputFolder = outputFolder;
            this.timestamps = timestamps;
        }

        public string ExecuteArgs() => "-y -ss " + timestamps.Peek().startTime + " -i \"" + inputMedia.FullName + "\" -c copy -t " + timestamps.Peek().endTime + " \"" + outputFolder + "\\" + inputMedia.Name + "\"";
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
