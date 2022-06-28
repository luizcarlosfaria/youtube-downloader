using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader;

public class MetadataDiscoveryPipelineActivity : IPipelineActivity
{
    private readonly DataService dataService;

    public MetadataDiscoveryPipelineActivity(DataService dataService)
    {
        this.dataService = dataService;
    }

    public async Task ExecuteAsync(Download download)
    {
        string title = await this.RunAsync("--get-title", download.OriginalMediaUrl);
        string thumbnail = await this.RunAsync("--get-thumbnail", download.OriginalMediaUrl);
        string description = await this.RunAsync("--get-description", download.OriginalMediaUrl);
        string durationRaw = await this.RunAsync("--get-duration", download.OriginalMediaUrl);
        TimeSpan duration = ParseDuration(durationRaw);

        this.dataService.Update(download.Id, (update) =>
             update.Combine(new[] {
                update.Set(it => it.Title, title),
                update.Set(it => it.ThumbnailUrl, thumbnail),
                update.Set(it => it.Description, description),
                update.Set(it => it.Duration, duration)
             })
         );
    }

    private static TimeSpan ParseDuration(string durationRaw)
    {
        if (string.IsNullOrWhiteSpace(durationRaw)) return TimeSpan.Zero;
        int foundSplitters = durationRaw.ToArray().Count(it => it == ':');
        for (int i = foundSplitters; i < 2; i++)
        {
            durationRaw = "00:" + durationRaw;
        }
        return TimeSpan.Parse(durationRaw);
    }

    private async Task<string> RunAsync(string action, string mediaUrl)
    {
        var process = System.Diagnostics.Process.Start(new ProcessStartInfo("yt-dlp", $"{action} {mediaUrl}")
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
