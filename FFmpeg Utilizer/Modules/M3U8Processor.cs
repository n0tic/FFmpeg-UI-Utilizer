using FFmpeg_Utilizer.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFmpeg_Utilizer.Modules
{
    public class M3U8Processor
    {
        public Main main;
        public Color backColor;
        internal bool inProcess = false;
        private M3U8ProcesserData processQueue;
        private CancellationTokenSource cancellationTokenSource;

        public M3U8Processor(Main _main)
        {
            this.main = _main;
            backColor = main.M3U8_listView.BackColor;
        }

        internal async Task ProcessFileQueue(M3U8ProcesserData data, bool multiDownload, int simultaneousDownloads = 1)
        {
            if (inProcess)
            {
                await StopProcessing();
                return;
            }

            if (!File.Exists(main.settings.ffmpegPath))
            {
                main.notice.SetNotice("You need to specify a location of \"ffmpeg.exe\" to use this function.", NoticeModule.TypeNotice.Error);
                return;
            }

            processQueue = data;
            inProcess = true;
            cancellationTokenSource = new CancellationTokenSource();

            main.M3U8_ProgressBar.Value = 0;
            main.M3U8_ProgressBar.Maximum = data.queue.Count;
            main.M3U8_StartButton.Text = "Stop M3U8";
            main.M3U8_StartButton.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);

            try
            {
                if (multiDownload)
                {
                    await ProcessFilesParallelAsync(simultaneousDownloads);
                }
                else
                {
                    await ProcessFilesSequentialAsync();
                }
            }
            finally
            {
                await main.InvokeAsync(() =>
                {
                    SystemSounds.Exclamation.Play();
                    main.M3U8_StartButton.Text = "Start M3U8";
                    main.M3U8_StartButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
                    inProcess = false;
                });
                cancellationTokenSource.Dispose();
            }
        }

        private async Task ProcessFilesParallelAsync(int simultaneousDownloads)
        {
            var semaphore = new SemaphoreSlim(simultaneousDownloads);
            var tasks = new List<Task>();

            while (processQueue.queue.Count > 0)
            {
                var itemData = processQueue.queue.Dequeue();
                var listItem = main.M3U8_listView.FindItemWithText(itemData.url);

                await main.InvokeAsync(() =>
                {
                    listItem.SubItems[2].Text = "Working...";
                });

                await semaphore.WaitAsync(cancellationTokenSource.Token);
                if (cancellationTokenSource.Token.IsCancellationRequested) break;

                tasks.Add(ProcessSingleFileAsync(itemData, listItem)
                    .ContinueWith(_ => semaphore.Release(), TaskContinuationOptions.ExecuteSynchronously));
            }

            await Task.WhenAll(tasks);
        }

        private async Task ProcessFilesSequentialAsync()
        {
            while (processQueue.queue.Count > 0 && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                var itemData = processQueue.queue.Dequeue();
                var listItem = main.M3U8_listView.FindItemWithText(itemData.url);

                await main.InvokeAsync(() =>
                {
                    listItem.SubItems[2].Text = "Working...";
                });

                await ProcessSingleFileAsync(itemData, listItem);
            }
        }

        private async Task ProcessSingleFileAsync(M3U8Argument itemData, ListViewItem listItem)
        {
            try
            {
                var outputFile = Path.Combine(processQueue.outputFolder, $"{itemData.name.Substring(0, Math.Min(100, itemData.name.Length))}.mp4");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = main.settings.ffmpegPath,
                        Arguments = $"-y -i \"{itemData.url}\" -c copy \"{outputFile}\" -report",
                        UseShellExecute = false,
                        CreateNoWindow = processQueue.hideConsole
                    }
                };

                await Task.Run(() =>
                {
                    process.Start();
                    process.WaitForExit();
                }, cancellationTokenSource.Token);

                FileInfo file = new FileInfo(outputFile);
                bool success = File.Exists(file.FullName) && file.Length > 0;

                await main.InvokeAsync(() =>
                {
                    listItem.SubItems[2].Text = success ? "✓ Finished" : "✗ Failed";
                    listItem.BackColor = success ? Color.FromArgb(192, 255, 192) : Color.FromArgb(255, 192, 192);
                    main.M3U8_ProgressBar.Value++;
                });
            }
            catch (Exception ex)
            {
                await main.InvokeAsync(() =>
                {
                    listItem.SubItems[2].Text = "✗ Failed";
                    listItem.BackColor = Color.FromArgb(255, 192, 192);
                    main.M3U8_ProgressBar.Value++;
                    main.notice.SetNotice(ex.Message, NoticeModule.TypeNotice.Error);
                });
            }
        }

        private async Task StopProcessing()
        {
            if (inProcess)
            {
                cancellationTokenSource.Cancel();

                foreach (ListViewItem item in main.M3U8_listView.Items)
                {
                    await main.InvokeAsync(() =>
                    {
                        switch (item.SubItems[2].Text)
                        {
                            case "Working...":
                                item.SubItems[2].Text = "✗ Aborted";
                                item.BackColor = Color.FromArgb(255, 192, 192);
                                break;
                            case "✗ Aborted":
                            case "✗ Failed":
                                item.SubItems[2].Text = "• Waiting";
                                item.BackColor = backColor;
                                break;
                        }
                    });
                }

                inProcess = false;
                main.M3U8_ProgressBar.Value = 0;
                main.M3U8_StartButton.Text = "Start M3U8";
                main.M3U8_StartButton.FlatAppearance.BorderColor = Color.FromArgb(99, 172, 229);
            }
        }
    }

    public static class ControlExtensions
    {
        public static Task InvokeAsync(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                var tcs = new TaskCompletionSource<bool>();
                control.Invoke(new Action(() =>
                {
                    try
                    {
                        action();
                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }));
                return tcs.Task;
            }
            else
            {
                action();
                return Task.CompletedTask;
            }
        }
    }
}