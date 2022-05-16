using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Minio;

namespace DevWeek.Services.Downloader
{
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

            await this.DownloadFromS3(download.MinioVideoStorage, videoLocalFullPath);

            string audioLocalFullPath = this.EncodeToMp3(download, videoLocalFullPath);

            download.MinioAudioStorage = new()
            {
                BucketName = this.AudioBucketName,
                ObjectName = Path.GetFileName(audioLocalFullPath)
            };

            await this.UploadToS3(download.MinioAudioStorage, audioLocalFullPath);

            await this.UpdateMetadata(download);

            Directory.Delete(newDirectory, true);
        }

        private async Task UpdateMetadata(Download download)
        {
            download.AudioDownloadUrl = $"/api/media/{download.MinioAudioStorage.BucketName}/download/{download.MinioAudioStorage.ObjectName}";

            await this.dataService.Update(download.Id, (update) =>
               update.Combine(new[] {
                    update.Set(it => it.AudioDownloadUrl, download.AudioDownloadUrl),
                    update.Set(it => it.MinioAudioStorage, download.MinioAudioStorage),
                    update.Set(it => it.Finished, DateTime.Now)
               })
           );
        }

        private string EncodeToMp3(Download download, string videoLocalFullPath)
        {
            string mp3FileName = Path.Combine(this.SharedPath, Path.Combine(download.Id, $"{download.Id}.mp3"));

            var processStartInfo = new ProcessStartInfo("ffmpeg", $"-i {videoLocalFullPath} {mp3FileName}");

            (string output, string error, int exitCode) = this.Run(processStartInfo);

            if (File.Exists(mp3FileName) == false)
                throw new InvalidOperationException("Operation is not completed " + output);

            return mp3FileName;
        }



    }

}
