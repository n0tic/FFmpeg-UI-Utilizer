using System.Collections.Generic;

namespace FFmpeg_Utilizer.Data
{
    [System.Serializable]
    public class M3U8ProcesserData
    {
        public string outputFolder;
        public bool hideConsole;
        public Queue<M3U8Argument> queue = new Queue<M3U8Argument>();

        public M3U8ProcesserData(string outputFolder, bool hideConsole)
        {
            this.outputFolder = outputFolder;
            this.hideConsole = hideConsole;
        }

        public void Add(string name, string url) => queue.Enqueue(new M3U8Argument(name, url));
    }
}