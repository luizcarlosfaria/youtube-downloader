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
            string bucketName = (string)context["defaultBucketName"];

            await minio.PutObjectAsync(bucketName, System.IO.Path.GetFileName(context.OutputFileName), context.OutputFilePath);

            string url = await minio.PresignedGetObjectAsync(bucketName, System.IO.Path.GetFileName(context.OutputFileName), (int)TimeSpan.FromHours(1).TotalSeconds);

            await this.dataService.Update(context.Download.Id, (update) =>
                update.Combine(new[] {
                    update.Set(it => it.DownloadUrl, url),
                    update.Set(it => it.Finished, DateTime.Now)
                })
            );
        }
    }
}
