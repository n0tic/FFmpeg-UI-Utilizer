namespace FFmpeg_Utilizer.Data
{
    public class MergeProcesserData
    {
        public string file = "";
        public string outputFolder = "";
        public string outputname = "";
        public string ext = "";
        public bool hideConsole = false;

        public MergeProcesserData(string file, string outputFolder, string outputname, string ext, bool hideConsole)
        {
            this.file = file;
            this.outputFolder = outputFolder;
            this.outputname = outputname;
            this.ext = ext;
            this.hideConsole = hideConsole;
        }
    }
}