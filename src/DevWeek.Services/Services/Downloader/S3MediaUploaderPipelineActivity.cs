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
        
        public S3MediaUploaderPipelineActivity(MinioClient minio)
        {
            this.minio = minio;
        }

        public async Task ExecuteAsync(Dictionary<string, string> context)
        {
            string bucketName = context["defaultBucketName"];
            string fileName = context["outputFilePath"];

            bool exists = await minio.BucketExistsAsync(bucketName);
            if (exists == false)
            {
                await minio.MakeBucketAsync(bucketName);
            }
            await minio.PutObjectAsync(bucketName, System.IO.Path.GetFileName(fileName), fileName);
        }
    }
}
