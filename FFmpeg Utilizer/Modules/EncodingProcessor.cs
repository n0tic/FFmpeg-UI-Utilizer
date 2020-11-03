using FFMPEG_Utilizer.Data;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;

/*
 * This is a stupid class that I wrote under duress. I apologize to all affected.
 * Those with heart conditions are advised to not continue
 *
 * ---> Because this is the reincarnation of a system I had to re-invent after an accidental deletion of this entire folder structure using SHIFT+DEL on windows.
 * I had no version control or backup. No File History or File Recovery could do it for me. Sadface - FML.
 */

namespace FFmpeg_Utilizer.Modules
{
    public class EncodingProcessor
    {
        public Main main;

        internal bool encodingInProcess = false;

        private EncodeProcesserData processQueue;

        internal Thread encodingThread;
        internal Thread queueThread;

        private Process encodingProcess;

        public EncodingProcessor(Main _main) => main = _main;

        internal void ProcessFileQueue(EncodeProcesserData data)
        {
            if (encodingInProcess)
            {
                try
                {
                    if (processQueue.queue.Count > 0)
                    {
                        ListView.ListViewItemCollection items = main.Encoder_FilesList.Items;
                        foreach (ListViewItem item in items)
                        {
                            switch (item.SubItems[1].Text)
                            {
                                case "¿ Processing":
                                    item.SubItems[1].Text = "✗ Aborted";
                                    break;

                                case "✗ Aborted":
                                    item.SubItems[1].Text = "• Waiting";
                                    break;

                                case "✗ Failed":
                                    item.SubItems[1].Text = "• Waiting";
                                    break;
                            }
                        }
                    }

                    encodingProcess.Kill();
                }
                catch (Win32Exception) { }
                catch (NotSupportedException) { }
                catch (InvalidOperationException) { }

                KillThreads();

                main.Encoder_ProgressBar.Value = 0;
                main.Encoder_StartEncodingProcessButton.Text = "Start Encoding";
                main.Encoder_StartEncodingProcessButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229); // 99, 172, 229 blue | 255, 128, 128 red
            }
            else
            {
                if (!File.Exists(main.settings.ffmpegPath))
                {
                    main.notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                    return;
                }

                processQueue = data;
                main.Encoder_ProgressBar.Value = 0;
                main.Encoder_ProgressBar.Maximum = data.queue.Count;

                queueThread = new Thread(() => StartQueueProcess());
                queueThread.Start();

                encodingInProcess = true;
                main.Encoder_StartEncodingProcessButton.Text = "Stop Encoding";
                main.Encoder_StartEncodingProcessButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
            }
        }

        private void StartQueueProcess()
        {
            bool queueProcess = true;
            while (queueProcess)
            {
                main.Invoke(new Action(() =>
                {
                    var item = main.Encoder_FilesList.FindItemWithText(processQueue.queue.Peek().inputFile.FullName);
                    item.SubItems[1].Text = "¿ Processing";
                }));

                encodingThread = new Thread(() => StartEncodingProcess());
                encodingThread.Start();

                //Give time for application to start. Is it needed?
                Thread.Sleep(100);

                while (encodingProcess != null) Thread.Sleep(25);

                FileInfo file = new FileInfo(processQueue.outputFolder + @"\" + processQueue.queue.Peek().outputName + "." + processQueue.queue.Peek().outputExtension);

                if (File.Exists(file.FullName))
                {
                    if (file.Length > 0)
                    {
                        main.Invoke(new Action(() =>
                        {
                            var item = main.Encoder_FilesList.FindItemWithText(processQueue.queue.Peek().inputFile.FullName);
                            item.SubItems[1].Text = "✓ Finished";
                        }));
                    }
                    else
                    {
                        main.Invoke(new Action(() =>
                        {
                            var item = main.Encoder_FilesList.FindItemWithText(processQueue.queue.Peek().inputFile.FullName);
                            item.SubItems[1].Text = "✗ Failed";
                        }));
                    }
                }
                else
                {
                    main.Invoke(new Action(() =>
                    {
                        var item = main.Encoder_FilesList.FindItemWithText(processQueue.queue.Peek().inputFile.FullName);
                        item.SubItems[1].Text = "✗ Failed";
                    }));
                }

                main.Invoke(new Action(() =>
                {
                    main.Encoder_ProgressBar.Value++;
                }));

                processQueue.queue.Dequeue();

                if (processQueue.queue.Count < 1) queueProcess = false;
            }

            main.Invoke(new Action(() =>
            {
                //Don't reset if successful.
                SystemSounds.Exclamation.Play();
                main.Encoder_StartEncodingProcessButton.Text = "Start Encoding";
                main.Encoder_StartEncodingProcessButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }));

            encodingInProcess = false;
        }

        private void StartEncodingProcess()
        {
            string args = processQueue.queue.Peek().ExecuteArgs();

            encodingProcess = new Process
            {
                StartInfo =
                {
                    FileName = main.settings.ffmpegPath,
                    Arguments = args,
                    UseShellExecute = false
                }
            };

            if (processQueue.hideConsole) encodingProcess.StartInfo.CreateNoWindow = true;

            try
            {
                encodingProcess.Start(); // ffmpegProcess.Id
                encodingProcess.WaitForExit();
            }
            catch (ObjectDisposedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (InvalidOperationException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (Win32Exception x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (PlatformNotSupportedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            finally { encodingProcess = null; }
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

            encodingInProcess = false;
        }
    }
}