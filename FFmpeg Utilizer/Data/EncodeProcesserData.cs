using FFmpeg_Utilizer.Data;
using System.Collections.Generic;
using System.IO;

namespace FFMPEG_Utilizer.Data
{
    public class EncodeProcesserData
    {
        public string outputFolder;
        public bool hideConsole;
        public Queue<EncoderArgument> queue = new Queue<EncoderArgument>();

        public EncodeProcesserData(string outputFolder, bool hideConsole)
        {
            this.outputFolder = outputFolder;
            this.hideConsole = hideConsole;
        }

        public void Add(Libs.Overwrite overwrite, FileInfo originalFile, Libs.VCodec vCodec, Libs.ACodec aCodec, Libs.Tune tuner, Libs.Preset preset, Libs.Frames fps, Libs.Size res, Libs.VideoFileExtensions ext) =>
            queue.Enqueue(new EncoderArgument(overwrite, originalFile, vCodec, aCodec, tuner, preset, fps, res, outputFolder, Path.GetFileNameWithoutExtension(originalFile.Name), ext));
    }
}