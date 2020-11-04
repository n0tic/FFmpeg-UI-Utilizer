using System;
using System.Collections.Generic;

namespace FFmpeg_Utilizer.Data
{
    public class Libs
    {
        #region System

        public enum Overwrite
        {
            Ask,
            Yes,
            No
        }

        #endregion System

        #region Frames

        public static string GetFramesCount(Frames frames)
        {
            if (frames != Frames.Default) return framesData[frames.ToString()];
            else return "";
        }

        private static readonly Dictionary<string, string> framesData = new Dictionary<string, string>() {
            { "FPS24", "24" },
            { "FPS23_976", "24.976" },
            { "FPS25", "25" },
            { "FPS29_97", "29.97" },
            { "FPS30", "30" },
            { "FPS50", "50" },
            { "FPS60", "60" }};

        public enum Frames
        {
            Default,
            FPS24,
            FPS23_976,
            FPS25,
            FPS29_97,
            FPS30,
            FPS50,
            FPS60
        }

        #endregion Frames

        #region Size - Width / Height

        public static string GetSize(Size size)
        {
            if (size != Size.Default) return sizeData[size.ToString()];
            else return "";
        }

        private static readonly Dictionary<string, string> sizeData = new Dictionary<string, string>() {
            { "sqcif", "128x96" },
            { "qqvga", "160x120" },
            { "qcif", "176x144" },
            { "hqvga", "240x160" },
            { "cga", "320x200" },
            { "qvga", "320x240" },
            { "qntsc", "352x240" },
            { "qpal", "352x288" },
            { "wqvga", "400x240" },
            { "hvga", "480x320" },
            { "ega", "640x350" },
            { "nhd", "640x360" },
            { "vga", "640x480" },
            { "ntsc", "720x480" },
            { "pal", "720x576" },
            { "spal", "768x576" },
            { "svga", "800x600" },
            { "hd480", "852x480" },
            { "qhd", "960x540" },
            { "xga", "1024x768" },
            { "sxga", "1280x1024" },
            { "hd720", "1280x720" },
            { "wxga", "1366x768" },
            { "uxga", "1600x1200" },
            { "wsxga", "1600x1024" },
            { "hd1080", "1920x1080" },
            { "wuxga", "1920x1200" },
            { "_2kflat", "1998x1080" },
            { "_2kscope", "2048x858" },
            { "_2k", "2048x1080" },
            { "qxga", "2048x1536" },
            { "woxga", "2560x1600" },
            { "qsxga", "2560x2048" },
            { "wqsxga", "3200x2048" },
            { "uhd2160", "3840x2160" },
            { "wquxga", "3840x2400" },
            { "_4kflat", "3996x2160" },
            { "_4kscope", "4096x1716" },
            { "_4k", "4096x2160" },
            { "hsxga", "5120x4096" },
            { "whsxga", "6400x4096" },
            { "uhd4320", "7680x4320" },
            { "whuxga", "7680x4800" }};

        public enum Size
        {
            Default,
            sqcif,
            qqvga,
            qcif,
            hqvga,
            cga,
            qvga,
            qntsc,
            qpal,
            wqvga,
            hvga,
            ega,
            nhd,
            vga,
            ntsc,
            pal,
            spal,
            svga,
            hd480,
            qhd,
            xga,
            sxga,
            hd720,
            wxga,
            uxga,
            wsxga,
            hd1080,
            wuxga,
            _2kflat,
            _2kscope,
            _2k,
            qxga,
            woxga,
            qsxga,
            wqsxga,
            uhd2160,
            wquxga,
            _4kflat,
            _4kscope,
            _4k,
            hsxga,
            whsxga,
            uhd4320,
            whuxga
        }

        #endregion Size - Width / Height

        #region Codec

        public enum VCodec
        {
            Default,
            copy,
            libx264, // Works with all args currently + aCodec
            libx265, // Bad quality? No args?!
            libxvid // Pretty fast, bad quality
        }

        public enum ACodec
        {
            Default,
            copy,
            aac,
            ac3,
            ac3_fixed,

            //opus, Experimental
            libopus
        }

        #endregion Codec

        #region Tuners / Presets

        public enum Tune
        {
            Default,
            film,
            animation,
            grain,
            stillimage,
            fastdecode,
            zerolatency,
            psnr, // – ignore this as it is only used for codec development
            ssim // – ignore this as it is only used for codec development
        }

        public enum Preset
        {
            Default,
            ultrafast,
            superfast,
            veryfast,
            faster,
            fast,
            slow,
            slower,
            veryslow,
        }

        #endregion Tuners / Presets

        #region Files

        public static List<string> GetAllowedExtension()
        {
            List<string> extensions = new List<string>();
            foreach (VideoFileExtensions ext in (VideoFileExtensions[])Enum.GetValues(typeof(VideoFileExtensions)))
                extensions.Add(ext.ToString());
            foreach (AudioFileExtensions ext in (AudioFileExtensions[])Enum.GetValues(typeof(AudioFileExtensions)))
                extensions.Add(ext.ToString());
            return extensions;
        }

        public enum VideoFileExtensions
        {
            mp4,
            avi,
            mov,
            flv,
            ts
        }

        public enum AudioFileExtensions
        {
            mp3,
            wav,
            aif,
            aiff
        }

        #endregion Files
    }
}