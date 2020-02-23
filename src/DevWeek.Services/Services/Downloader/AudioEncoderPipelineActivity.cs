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
    public class AudioEncoderPipelineActivity : MediaBaseActivity, IPipelineActivity
    {

        public Task ExecuteAsync(DownloadContext context)
        {

            string defaultDownloadPath = Path.Combine(this.SharedPath, $"{context.Download.Id}.mp3");

            var processStartInfo = new ProcessStartInfo("ffmpeg", $"-i {context.VideoOutputFilePath} {defaultDownloadPath}");

            (string standardOutput, _) = this.Run(processStartInfo);

            context.AudioOutputFilePath = this.ExtractPath(defaultDownloadPath, standardOutput);

            return Task.CompletedTask;
        }

    }

}
