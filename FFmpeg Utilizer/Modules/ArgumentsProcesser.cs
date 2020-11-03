using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;

namespace FFmpeg_Utilizer.Modules
{
    public class ArgumentsProcesser
    {
        public Main main;

        internal bool inProcess = false;

        internal Thread argsWorkerThread;
        internal Thread checkerThread;

        private string args = "";

        private Process argsProcess;

        public ArgumentsProcesser(Main _main) => this.main = _main;

        internal void ProcessArgs(string args)
        {
            if (inProcess)
            {
                try
                {
                    argsProcess.Kill();
                }
                catch (Win32Exception) { }
                catch (NotSupportedException) { }
                catch (InvalidOperationException) { }

                KillThreads();

                main.Argument_RunArgumentButton.Text = "Run Argument ▶";
                main.Argument_RunArgumentButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229); // 99, 172, 229 blue | 255, 128, 128 red
            }
            else
            {
                if (!File.Exists(main.settings.ffmpegPath))
                {
                    main.notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                    return;
                }

                this.args = args;

                checkerThread = new Thread(() => StartQueueProcess());
                checkerThread.Start();

                inProcess = true;
                main.Argument_RunArgumentButton.Text = "Stop Argument";
                main.Argument_RunArgumentButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
            }
        }

        private void StartQueueProcess()
        {
            argsWorkerThread = new Thread(() => StartEncodingProcess());
            argsWorkerThread.Start();

            //Give time for application to start. Is it needed?
            Thread.Sleep(100);

            while (argsProcess != null) Thread.Sleep(25);

            main.Invoke(new Action(() =>
            {
                //Don't reset if successful.
                SystemSounds.Exclamation.Play();
                main.Argument_RunArgumentButton.Text = "Run Argument ▶";
                main.Argument_RunArgumentButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }));

            inProcess = false;
        }

        private void StartEncodingProcess()
        {
            argsProcess = new Process
            {
                StartInfo =
                {
                    FileName = main.settings.ffmpegPath,
                    Arguments = args,
                    UseShellExecute = false
                }
            };

            try
            {
                argsProcess.Start(); // ffmpegProcess.Id
                argsProcess.WaitForExit();
            }
            catch (ObjectDisposedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (InvalidOperationException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (Win32Exception x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (PlatformNotSupportedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            finally { argsProcess = null; }
        }

        internal void KillThreads()
        {
            try
            {
                checkerThread?.Abort();
                argsWorkerThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (System.Security.SecurityException) { }
            catch (ThreadStateException) { }

            inProcess = false;
        }
    }
}