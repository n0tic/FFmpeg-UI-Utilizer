using FFmpeg_Utilizer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFmpeg_Utilizer.Modules
{
    public class CutProcesser
    {
        public Main main;

        internal bool inProcess = false;

        private CutArgument processQueue;

        internal Thread encodingThread;
        internal Thread queueThread;

        private Process cutProcess;

        public CutProcesser(Main _main) => this.main = _main;
    }
}
