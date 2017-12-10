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

        private readonly DownloadUpdateService metadataUpdater;

        public S3MediaUploaderPipelineActivity(MinioClient minio, DownloadUpdateService metadataUpdater)
        {
            this.metadataUpdater = metadataUpdater;
            this.minio = minio;
        }

        public async Task ExecuteAsync(DownloadContext context)
        {
            string bucketName = (string)context["defaultBucketName"];

            await minio.PutObjectAsync(bucketName, System.IO.Path.GetFileName(context.OutputFileName), context.OutputFilePath);

            string url = await minio.PresignedGetObjectAsync(bucketName, System.IO.Path.GetFileName(context.OutputFileName), (int)TimeSpan.FromHours(1).TotalSeconds);

            this.metadataUpdater.Update(context.MediaUrl, (download) =>
            {
                download.DownloadUrl = url;
                download.Finished = DateTime.Now;
            });
        }
    }
}
