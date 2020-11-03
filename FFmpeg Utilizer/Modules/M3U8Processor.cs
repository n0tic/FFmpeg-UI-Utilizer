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
    public class M3U8Processor
    {
        public Main main;

        internal bool inProcess = false;

        private M3U8ProcesserData processQueue;

        internal Thread encodingThread;
        internal Thread queueThread;

        private Process m3u8Process;

        public M3U8Processor(Main _main) => this.main = _main;

        internal void ProcessFileQueue(M3U8ProcesserData data)
        {
            if (inProcess)
            {
                try
                {
                    if (processQueue.queue.Count > 0)
                    {
                        ListView.ListViewItemCollection items = main.M3U8_listView.Items;
                        foreach (ListViewItem item in items)
                        {
                            switch (item.SubItems[2].Text)
                            {
                                case "¿ Processing":
                                    item.SubItems[2].Text = "✗ Aborted";
                                    break;

                                case "✗ Aborted":
                                    item.SubItems[2].Text = "• Waiting";
                                    break;

                                case "✗ Failed":
                                    item.SubItems[2].Text = "• Waiting";
                                    break;
                            }
                        }
                    }

                    m3u8Process.Kill();
                }
                catch (Win32Exception) { }
                catch (NotSupportedException) { }
                catch (InvalidOperationException) { }

                KillThreads();

                main.M3U8_ProgressBar.Value = 0;
                main.M3U8_StartButton.Text = "Start M3U8";
                main.M3U8_StartButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229); // 99, 172, 229 blue | 255, 128, 128 red
            }
            else
            {
                if (!File.Exists(main.settings.ffmpegPath))
                {
                    main.notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                    return;
                }

                processQueue = data;
                main.M3U8_ProgressBar.Value = 0;
                main.M3U8_ProgressBar.Maximum = data.queue.Count;

                queueThread = new Thread(() => StartQueueProcess());
                queueThread.Start();

                inProcess = true;
                main.M3U8_StartButton.Text = "Stop M3U8";
                main.M3U8_StartButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
            }
        }

        private void StartQueueProcess()
        {
            bool queueProcess = true;
            while (queueProcess)
            {
                main.Invoke(new Action(() =>
                {
                    var item = main.M3U8_listView.FindItemWithText(processQueue.queue.Peek().url);
                    item.SubItems[2].Text = "¿ Processing";
                }));

                encodingThread = new Thread(() => StartEncodingProcess());
                encodingThread.Start();

                //Give time for application to start. Is it needed?
                Thread.Sleep(100);

                while (m3u8Process != null) Thread.Sleep(25);

                FileInfo file = new FileInfo(processQueue.outputFolder + @"\" + processQueue.queue.Peek().name + ".mp4");

                if (File.Exists(file.FullName))
                {
                    if (file.Length > 0)
                    {
                        main.Invoke(new Action(() =>
                        {
                            var item = main.M3U8_listView.FindItemWithText(processQueue.queue.Peek().url);
                            item.SubItems[2].Text = "✓ Finished";
                        }));
                    }
                    else
                    {
                        main.Invoke(new Action(() =>
                        {
                            var item = main.M3U8_listView.FindItemWithText(processQueue.queue.Peek().url);
                            item.SubItems[2].Text = "✗ Failed";
                        }));
                    }
                }
                else
                {
                    main.Invoke(new Action(() =>
                    {
                        var item = main.M3U8_listView.FindItemWithText(processQueue.queue.Peek().url);
                        item.SubItems[2].Text = "✗ Failed";
                    }));
                }

                main.Invoke(new Action(() =>
                {
                    main.M3U8_ProgressBar.Value++;
                }));

                processQueue.queue.Dequeue();

                if (processQueue.queue.Count < 1) queueProcess = false;
            }

            main.Invoke(new Action(() =>
            {
                //Don't reset if successful.
                SystemSounds.Exclamation.Play();
                main.M3U8_StartButton.Text = "Start M3U8";
                main.M3U8_StartButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }));

            inProcess = false;
        }

        private void StartEncodingProcess()
        {
            m3u8Process = new Process
            {
                StartInfo =
                {
                    FileName = main.settings.ffmpegPath,
                    Arguments = "-y -i \"" + processQueue.queue.Peek().url + "\" -absf aac_adtstoasc -acodec copy -vcodec copy \"" + processQueue.outputFolder + "\\" + processQueue.queue.Peek().name + ".mp4",
                    UseShellExecute = false
                }
            };

            if (processQueue.hideConsole) m3u8Process.StartInfo.CreateNoWindow = true;

            try
            {
                m3u8Process.Start(); // ffmpegProcess.Id
                m3u8Process.WaitForExit();
            }
            catch (ObjectDisposedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (InvalidOperationException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (Win32Exception x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            catch (PlatformNotSupportedException x) { main.notice.SetNotice(x.Message, NoticeModule.TypeNotice.Error); }
            finally { m3u8Process = null; }
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