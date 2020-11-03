using FFmpeg_Utilizer.Data;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Modules
{
    public class CutProcessor
    {
        public Main main;

        internal bool inProcess = false;

        private CutArgument processQueue;

        internal Thread encodingThread;
        internal Thread queueThread;

        private Process cutProcess;

        public CutProcessor(Main _main) => this.main = _main;

        internal void ProcessFileQueue(CutArgument data)
        {
            if (inProcess)
            {
                try
                {
                    cutProcess.Kill();
                }
                catch (Win32Exception) { }
                catch (NotSupportedException) { }
                catch (InvalidOperationException) { }

                KillThreads();

                main.Cut_ProgressBar.Value = 0;
                main.Cut_StartCuttingButton.Text = "Start Cutting";
                main.Cut_StartCuttingButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229); // 99, 172, 229 blue | 255, 128, 128 red
            }
            else
            {
                if (!File.Exists(main.settings.ffmpegPath))
                {
                    main.notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                    return;
                }

                processQueue = data;
                main.Cut_ProgressBar.Value = 0;
                main.Cut_ProgressBar.Maximum = data.timestamps.Count;

                queueThread = new Thread(() => StartQueueProcess());
                queueThread.Start();

                inProcess = true;
                main.Cut_StartCuttingButton.Text = "Stop Cutting";
                main.Cut_StartCuttingButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
            }
        }

        private void StartQueueProcess()
        {
            bool queueProcess = true;
            while (queueProcess)
            {
                encodingThread = new Thread(() => StartEncodingProcess());
                encodingThread.Start();

                //Give time for application to start. Is it needed?
                Thread.Sleep(100);

                while (cutProcess != null) Thread.Sleep(25);

                main.Invoke(new Action(() =>
                {
                    main.Cut_ProgressBar.Value++;
                }));

                processQueue.timestamps.Dequeue();

                if (processQueue.timestamps.Count < 1) queueProcess = false;
            }

            main.Invoke(new Action(() =>
            {
                //Don't reset if successful.
                SystemSounds.Exclamation.Play();
                main.Cut_StartCuttingButton.Text = "Start Cutting";
                main.Cut_StartCuttingButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }));

            inProcess = false;
        }

        private void StartEncodingProcess()
        {
            main.Invoke(new Action(() =>
            {
                Clipboard.SetText(processQueue.ExecuteArgs());
            }));

            cutProcess = new Process
            {
                StartInfo =
                {
                    FileName = main.settings.ffmpegPath,
                    Arguments = processQueue.ExecuteArgs(),
                    UseShellExecute = false
                }
            };

            if (main.Cut_HideConsoleToggle.Checked) cutProcess.StartInfo.CreateNoWindow = true;

            try
            {
                cutProcess.Start(); // ffmpegProcess.Id
                cutProcess.WaitForExit();
            }
            catch (ObjectDisposedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (InvalidOperationException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (Win32Exception x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (PlatformNotSupportedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            finally { cutProcess = null; }
        }

        internal void KillThreads()
        {
            try
            {
                queueThread?.Abort();
                encodingThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (System.Security.SecurityException) { }
            catch (ThreadStateException) { }

            inProcess = false;
        }
    }
}