﻿using DevWeek.Architecture.FluentRunner;
using Minio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader;

public class AudioEncoderPipelineActivity : MediaBaseActivity, IPipelineActivity
{
    private readonly MinioClient minio;

    private readonly DataService dataService;

    public string AudioBucketName { get; set; }

    public AudioEncoderPipelineActivity(MinioClient minio, DataService dataService) : base(minio)
    {
        this.dataService = dataService;
        this.minio = minio;
    }


    public async Task ExecuteAsync(Download download)
    {
        string newDirectory = this.TryCreateDownloadDirectory(download);

        string videoLocalFullPath = Path.Combine(newDirectory, download.MinioVideoStorage.ObjectName);

        await this.DownloadFromS3Async(download.MinioVideoStorage, videoLocalFullPath);

        string audioLocalFullPath = await this.EncodeToMp3Async(download, videoLocalFullPath);

        download.MinioAudioStorage = new()
        {
            BucketName = this.AudioBucketName,
            ObjectName = Path.GetFileName(audioLocalFullPath)
        };

        await this.UploadToS3Async(download.MinioAudioStorage, audioLocalFullPath);

        this.UpdateMetadata(download);

        Directory.Delete(newDirectory, true);
    }

    private void UpdateMetadata(Download download)
    {
        download.AudioDownloadUrl = $"/api/media/{download.MinioAudioStorage.BucketName}/download/{download.MinioAudioStorage.ObjectName}";

        this.dataService.Update(download.Id, (update) =>
           update.Combine(new[] {
                update.Set(it => it.AudioDownloadUrl, download.AudioDownloadUrl),
                update.Set(it => it.MinioAudioStorage, download.MinioAudioStorage),
                update.Set(it => it.Finished, DateTime.Now)
           })
       );
    }

    private async Task<string> EncodeToMp3Async(Download download, string videoLocalFullPath)
    {
        string mp3FileName = Path.Combine(this.SharedPath, Path.Combine(download.Id, $"{download.Id}.mp3"));

        ExecutionResult executionResult = await (new RunnerBuider()
            .Process("ffmpeg")
            .Arg($"-i {videoLocalFullPath}")
            .Arg($"{mp3FileName}")
            .RunAsync());

        if (File.Exists(mp3FileName) == false)
            throw new InvalidOperationException("Operation is not completed " + executionResult.StandardOutput);

        return mp3FileName;
    }



}

