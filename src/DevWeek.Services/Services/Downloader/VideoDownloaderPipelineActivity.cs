using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DevWeek.Services.Downloader.MediaDownloader;

namespace DevWeek.Services.Downloader
{
    public class VideoDownloaderPipelineActivity : MediaBaseActivity, IPipelineActivity
    {


        public Task ExecuteAsync(DownloadContext context)
        {
            string defaultDownloadPath = Path.Combine(this.SharedPath, $"{context.Download.Id}.mp4");

            var processStartInfo = new ProcessStartInfo("yt-dlp", $"-o {defaultDownloadPath} {context.Download.OriginalMediaUrl}");

            (string output, string error, int exitCode) = this.Run(processStartInfo);

            context.VideoOutputFilePath = this.ExtractPath(defaultDownloadPath, output);

            return Task.CompletedTask;
        }

    }

}
