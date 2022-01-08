using Minio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public class S3MediaUploaderPipelineActivity : IPipelineActivity
    {
        private readonly MinioClient minio;

        private readonly DataService dataService;

        public string AudioBucketName { get; set; }

        public string VideoBucketName { get; set; }


        public S3MediaUploaderPipelineActivity(MinioClient minio, DataService dataService)
        {
            this.dataService = dataService;
            this.minio = minio;
        }

        public async Task ExecuteAsync(DownloadContext context)
        {
            string audioFileName = System.IO.Path.GetFileName(context.AudioOutputFilePath);
            await minio.PutObjectAsync(this.AudioBucketName, audioFileName, context.AudioOutputFilePath);


            string videoFileName = System.IO.Path.GetFileName(context.VideoOutputFilePath);
            await minio.PutObjectAsync(this.VideoBucketName, videoFileName, context.VideoOutputFilePath);


            await this.dataService.Update(context.Download.Id, (update) =>
                update.Combine(new[] {                
                    update.Set(it => it.AudioDownloadUrl, $"/api/media/{this.AudioBucketName}/download/{audioFileName}"),
                    update.Set(it => it.VideoDownloadUrl, $"/api/media/{this.VideoBucketName}/download/{videoFileName}"),
                    update.Set(it => it.PlayUrl, $"/api/media/{this.VideoBucketName}/stream/{videoFileName}"),
                    update.Set(it => it.Finished, DateTime.Now)
                })
            );
        }
    }
}
