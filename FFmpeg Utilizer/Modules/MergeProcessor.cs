using FFmpeg_Utilizer.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Modules
{
    public class MergeProcessor
    {
        public Main main;

        internal bool inProcess = false;

        private MergeProcesserData data;

        internal Thread mergeWorkerThread;
        internal Thread checkerThread;

        private Process mergeProcess;

        public MergeProcessor(Main _main) => this.main = _main;

        internal void ProcessMerge(MergeProcesserData data)
        {
            if (inProcess)
            {
                try
                {
                    mergeProcess.Kill();
                }
                catch (Win32Exception) { }
                catch (NotSupportedException) { }
                catch (InvalidOperationException) { }

                KillThreads();

                main.Merge_ProgressBar.Value = 0;
                main.Merge_StartButton.Text = "Start Merging";
                main.Merge_StartButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229); // 99, 172, 229 blue | 255, 128, 128 red
            }
            else
            {
                if (!File.Exists(main.settings.ffmpegPath))
                {
                    main.notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                    return;
                }

                this.data = data;
                main.Merge_ProgressBar.Value = 0;
                main.Merge_ProgressBar.Maximum = 1;

                checkerThread = new Thread(() => StartQueueProcess());
                checkerThread.Start();

                inProcess = true;
                main.Merge_StartButton.Text = "Stop Merging";
                main.Merge_StartButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
            }
        }

        private void StartQueueProcess()
        {
            mergeWorkerThread = new Thread(() => StartEncodingProcess());
            mergeWorkerThread.Start();

            //Give time for application to start. Is it needed?
            Thread.Sleep(100);

            while (mergeProcess != null) Thread.Sleep(25);

            main.Invoke(new Action(() =>
            {
                //Don't reset if successful.
                SystemSounds.Exclamation.Play();
                main.M3U8_StartButton.Text = "Start Merging";
                main.M3U8_StartButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }));

            inProcess = false;
        }

        private void StartEncodingProcess()
        {
            mergeProcess = new Process
            {
                StartInfo =
                {
                    FileName = main.settings.ffmpegPath,
                    Arguments = "-y -f concat -safe 0 -i \"" + data.file + "\" -c copy \"" + data.outputFolder + "\\" + data.outputname + data.ext + "\"",
                    UseShellExecute = false
                }
            };

            if (data.hideConsole) mergeProcess.StartInfo.CreateNoWindow = true;

            try
            {
                mergeProcess.Start(); // ffmpegProcess.Id
                mergeProcess.WaitForExit();
            }
            catch (ObjectDisposedException x) { MessageBox.Show(x.Message); }
            catch (InvalidOperationException x) { MessageBox.Show(x.Message); }
            catch (Win32Exception x) { MessageBox.Show(x.Message); }
            catch (PlatformNotSupportedException x) { MessageBox.Show(x.Message); }
            finally { mergeProcess = null; }
        }

        internal void KillThreads()
        {
            try
            {
                checkerThread?.Abort();
                mergeWorkerThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (System.Security.SecurityException) { }
            catch (ThreadStateException) { }

            inProcess = false;
        }
    }
}
