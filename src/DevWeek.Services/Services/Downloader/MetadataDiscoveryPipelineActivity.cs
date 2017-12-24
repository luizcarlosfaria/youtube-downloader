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
        private readonly DataService dataService;

        public MetadataDiscoveryPipelineActivity(DataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task ExecuteAsync(DownloadContext context)
        {
            string title = await RunAsync("--get-title", context.MediaUrl);
            string thumbnail = await RunAsync("--get-thumbnail", context.MediaUrl);
            string description = await RunAsync("--get-description", context.MediaUrl);
            string durationRaw = await RunAsync("--get-duration", context.MediaUrl);
            TimeSpan duration = this.ParseDuration(durationRaw);

            await this.dataService.Update(context.Download.Id, (update) =>
                 update.Combine(new[] {
                    update.Set(it => it.Title, title),
                    update.Set(it => it.ThumbnailUrl, thumbnail),
                    update.Set(it => it.Description, description),
                    update.Set(it => it.Duration, duration)
                 })
             );
        }

        private TimeSpan ParseDuration(string durationRaw)
        {
            int foundSplitters = durationRaw.ToArray().Count(it => it == ':');
            for (int i = foundSplitters; i < 2; i++)
            {
                durationRaw = "00:" + durationRaw;
            }
            return TimeSpan.Parse(durationRaw);
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
