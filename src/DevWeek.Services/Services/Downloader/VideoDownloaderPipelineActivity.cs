using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Minio;
using DevWeek.Architecture.FluentRunner;

namespace DevWeek.Services.Downloader;

public class VideoDownloaderPipelineActivity : MediaBaseActivity, IPipelineActivity
{

    private readonly DataService dataService;

    public string VideoBucketName { get; set; }

    public VideoDownloaderPipelineActivity(MinioClient minio, DataService dataService) : base(minio)
    {
        this.dataService = dataService;
    }

    public async Task ExecuteAsync(Download download)
    {
        string newDirectory = this.TryCreateDownloadDirectory(download);

        string videoLocalFullPath = await this.DownloadFromYoutubeToLocalFileSystemAsync(download, newDirectory);

        download.MinioVideoStorage = new()
        {
            BucketName = this.VideoBucketName,
            ObjectName = Path.GetFileName(videoLocalFullPath)
        };

        await this.UploadToS3Async(download.MinioVideoStorage, videoLocalFullPath);

        this.UpdateMetadata(download);

        Directory.Delete(newDirectory, true);
    }

    private void UpdateMetadata(Download download)
    {
        download.VideoDownloadUrl = $"/api/media/{download.MinioVideoStorage.BucketName}/download/{download.MinioVideoStorage.ObjectName}";
        download.PlayUrl = $"/api/media/{download.MinioVideoStorage.BucketName}/stream/{download.MinioVideoStorage.ObjectName}";

        this.dataService.Update(download.Id, (update) =>
           update.Combine(new[] {
                update.Set(it => it.VideoDownloadUrl, download.VideoDownloadUrl),
                update.Set(it => it.PlayUrl, download.PlayUrl),
                update.Set(it => it.MinioVideoStorage, download.MinioVideoStorage),
           })
       );
    }

    private async Task<string> DownloadFromYoutubeToLocalFileSystemAsync(Download download, string newDirectory)
    {
        string mediaFilePath = Path.Combine(newDirectory, $"{download.Id}.mp4");

        ExecutionResult executionResult  = await (new RunnerBuider()
            .Process("yt-dlp")
            .Arg($"--output {mediaFilePath}")
            .Arg($"--format mp4")
            .Arg($"{download.OriginalMediaUrl}")
            .RunAsync());

        return Directory.GetFiles(newDirectory).Single();
    }




}

