using FFmpeg_Utilizer.Data;
using System.Collections.Generic;
using System.IO;

namespace FFMPEG_Utilizer.Data
{
    public class NormalizeAudioData
    {
        public string outputFolder;
        public bool hideConsole;
        public Queue<FileInfo> queue = new Queue<FileInfo>();

        public NormalizeAudioData(string outputFolder, bool hideConsole)
        {
            this.outputFolder = outputFolder;
            this.hideConsole = hideConsole;
        }

        public void Add(string text) => queue.Enqueue(new FileInfo(text));
    }
}