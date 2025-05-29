using FFMPEG_Utilizer.Data;
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
    public class NormalizeAudioProcesser
    {
        public Main main;

        internal bool normalizeAudioInProcess = false;

        private NormalizeAudioData processQueue;

        internal Thread queueThread;

        private Process normalizeAudioProcess;

        public NormalizeAudioProcesser(Main _main) => main = _main;

        internal void ProcessFileQueue(NormalizeAudioData data)
        {
            if (normalizeAudioInProcess)
            {
                try
                {
                    if (processQueue.queue.Count > 0)
                    {
                        ListView.ListViewItemCollection items = main.NormalizeAudio_ListView.Items;
                        foreach (ListViewItem item in items)
                        {
                            switch (item.SubItems[1].Text)
                            {
                                case "Working...":
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

                    normalizeAudioProcess?.Kill();
                }
                catch (Win32Exception) { }
                catch (NotSupportedException) { }
                catch (InvalidOperationException) { }

                KillThreads();

                main.NormalizeAudio_Progressbar.Value = 0;
                main.NormalizeAudio_StartNormalizingAudioButton.Text = "Start Process";
                main.NormalizeAudio_StartNormalizingAudioButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }
            else
            {
                if (!File.Exists(main.settings.ffmpegPath))
                {
                    main.notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                    return;
                }

                // Ensure output folder exists
                if (!Directory.Exists(data.outputFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(data.outputFolder);
                    }
                    catch (Exception ex)
                    {
                        main.notice.SetNotice($"Failed to create output directory: {ex.Message}", NoticeModule.TypeNotice.Error);
                        return;
                    }
                }

                processQueue = data;
                main.NormalizeAudio_Progressbar.Value = 0;
                main.NormalizeAudio_Progressbar.Maximum = data.queue.Count;

                queueThread = new Thread(() => StartQueueProcess());
                queueThread.Start();

                normalizeAudioInProcess = true;
                main.NormalizeAudio_StartNormalizingAudioButton.Text = "Stop Process";
                main.NormalizeAudio_StartNormalizingAudioButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
            }
        }

        private void StartQueueProcess()
        {
            while (processQueue.queue.Count > 0)
            {
                FileInfo currentFile = processQueue.queue.Peek();
                string inputPath = currentFile.FullName;
                string outputPath = Path.Combine(processQueue.outputFolder, currentFile.Name);

                main.Invoke(new Action(() =>
                {
                    var item = main.NormalizeAudio_ListView.FindItemWithText(inputPath);
                    if (item != null)
                    {
                        item.SubItems[1].Text = "Working...";
                    }
                }));

                StartEncodingProcess(currentFile);

                if (File.Exists(outputPath))
                {
                    FileInfo outputFile = new FileInfo(outputPath);
                    if (outputFile.Length > 0)
                    {
                        main.Invoke(new Action(() =>
                        {
                            var item = main.NormalizeAudio_ListView.FindItemWithText(inputPath);
                            if (item != null)
                            {
                                item.SubItems[1].Text = "✓ Finished";
                                item.SubItems[1].BackColor = Color.FromArgb(192, 255, 192);
                            }
                        }));
                    }
                    else
                    {
                        main.Invoke(new Action(() =>
                        {
                            var item = main.NormalizeAudio_ListView.FindItemWithText(inputPath);
                            if (item != null)
                            {
                                item.SubItems[1].Text = "✗ Failed";
                                item.SubItems[1].BackColor = Color.FromArgb(255, 192, 192);
                            }
                        }));
                    }
                }
                else
                {
                    main.Invoke(new Action(() =>
                    {
                        var item = main.NormalizeAudio_ListView.FindItemWithText(inputPath);
                        if (item != null)
                        {
                            item.SubItems[1].Text = "✗ Failed";
                            item.SubItems[1].BackColor = Color.FromArgb(255, 192, 192);
                        }
                    }));
                }

                main.Invoke(new Action(() =>
                {
                    main.NormalizeAudio_Progressbar.Value++;
                }));

                processQueue.queue.Dequeue();
            }

            main.Invoke(new Action(() =>
            {
                SystemSounds.Exclamation.Play();
                main.NormalizeAudio_StartNormalizingAudioButton.Text = "Start Process";
                main.NormalizeAudio_StartNormalizingAudioButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }));

            normalizeAudioInProcess = false;
        }

        private void StartEncodingProcess(FileInfo inputFile)
        {
            string inputPath = inputFile.FullName;
            string outputPath = Path.Combine(processQueue.outputFolder, inputFile.Name);
            string args = $"-y -i \"{inputPath}\" -c:v copy -filter:a loudnorm -c:a aac \"{outputPath}\"";

            normalizeAudioProcess = new Process
            {
                StartInfo =
                {
                    FileName = main.settings.ffmpegPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = processQueue.hideConsole
                }
            };

            try
            {
                normalizeAudioProcess.Start();
                normalizeAudioProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                main.Invoke(new Action(() =>
                {
                    main.notice.SetNotice($"FFmpeg error: {ex.Message}", NoticeModule.TypeNotice.Error);
                }));
            }
            finally
            {
                normalizeAudioProcess = null;
            }
        }

        internal void KillThreads()
        {
            try
            {
                queueThread?.Abort();
            }
            catch (PlatformNotSupportedException) { }
            catch (System.Security.SecurityException) { }
            catch (ThreadStateException) { }

            normalizeAudioInProcess = false;
        }
    }
}