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

        public S3MediaUploaderPipelineActivity(MinioClient minio, DataService dataService)
        {
            this.dataService = dataService;
            this.minio = minio;
        }

        public async Task ExecuteAsync(DownloadContext context)
        {
            string audioBucketName = (string)context["audioBucketName"];
            string audioFileName = System.IO.Path.GetFileName(context.AudioOutputFilePath);
            await minio.PutObjectAsync(audioBucketName, audioFileName, context.AudioOutputFilePath);


            string videobucketName = (string)context["videoBucketName"];
            string videoFileName = System.IO.Path.GetFileName(context.VideoOutputFilePath);
            await minio.PutObjectAsync(videobucketName, videoFileName, context.VideoOutputFilePath);


            await this.dataService.Update(context.Download.Id, (update) =>
                update.Combine(new[] {
                    update.Set(it => it.AudioDownloadUrl, $"/api/media/{audioBucketName}/download/{audioFileName}"),
                    update.Set(it => it.VideoDownloadUrl, $"/api/media/{videobucketName}/download/{videoFileName}"),
                    update.Set(it => it.PlayUrl, $"/api/media/{videobucketName}/stream/{videoFileName}"),
                    update.Set(it => it.Finished, DateTime.Now)
                })
            );
        }
    }
}
