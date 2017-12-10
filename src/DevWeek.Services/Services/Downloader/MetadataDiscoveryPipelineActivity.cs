using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public class MetadataDiscoveryPipelineActivity : IPipelineActivity
    {
        private readonly DownloadUpdateService metadataUpdater;

        public MetadataDiscoveryPipelineActivity(DownloadUpdateService metadataUpdater) {
            this.metadataUpdater = metadataUpdater;
        }

        public async Task ExecuteAsync(DownloadContext context)
        {
            string title = await RunAsync("--get-title", context.MediaUrl);
            string thumbnail = await RunAsync("--get-thumbnail", context.MediaUrl);
            string description = await RunAsync("--get-description", context.MediaUrl);
            TimeSpan duration = TimeSpan.Parse(await RunAsync("--get-duration", context.MediaUrl));

            this.metadataUpdater.Update(context.MediaUrl, (download) =>
            {
                download.Title = title;
                download.ThumbnailUrl = thumbnail;
                download.Description = description;
                download.Duration = duration;
            });
        }

        private async Task<string> RunAsync(string action, string mediaUrl)
        {
            var process = System.Diagnostics.Process.Start(new ProcessStartInfo("youtube-dl", $"{action} {mediaUrl}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process.WaitForExit();

            string standardError = await process.StandardError.ReadToEndAsync();
            string standardOutput = await process.StandardOutput.ReadToEndAsync();

            if (process.ExitCode != 0)
            {
                throw new ApplicationException("Download Failure", new Exception(standardError + standardOutput));
            }

            return standardOutput.Replace(System.Environment.NewLine, string.Empty);
        }
    }
}
