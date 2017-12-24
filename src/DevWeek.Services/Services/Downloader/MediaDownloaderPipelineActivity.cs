using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public class MediaDownloaderPipelineActivity : IPipelineActivity
    {
        public async Task ExecuteAsync(DownloadContext context)
        {
            await VideoDownload(context);
            await AudioDownload(context);
        }

        private static async Task VideoDownload(DownloadContext context)
        {
            var process = System.Diagnostics.Process.Start(new ProcessStartInfo("youtube-dl", $"-o {context.VideoOutputFilePath} {context.MediaUrl}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                //Console.WriteLine(await process.StandardError.ReadToEndAsync());
                //Console.WriteLine(await process.StandardOutput.ReadToEndAsync());
                throw new ApplicationException("Download Failure", new Exception(await process.StandardError.ReadToEndAsync() + await process.StandardOutput.ReadToEndAsync()));
            }
        }

        private static async Task AudioDownload(DownloadContext context)
        {
            var process = System.Diagnostics.Process.Start(new ProcessStartInfo("ffmpeg", $"-i {context.VideoOutputFilePath} {context.AudioOutputFilePath}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                //Console.WriteLine(await process.StandardError.ReadToEndAsync());
                //Console.WriteLine(await process.StandardOutput.ReadToEndAsync());
                throw new ApplicationException("Download Failure", new Exception(await process.StandardError.ReadToEndAsync() + await process.StandardOutput.ReadToEndAsync()));
            }
        }
    }
}
