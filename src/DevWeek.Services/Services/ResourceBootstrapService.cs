using Minio;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Services
{
    public class ResourceBootstrapService
    {
        private readonly MinioClient minio;
        private readonly RabbitMQ.Client.IModel model;

        public string MinioBucketName { get; set; }
        public string DownloadPipelineQueue { get; set; }
        public string DownloadPipelineRouteKey { get; set; }
        public string DownloadPipelineExchange { get; set; }

        public ResourceBootstrapService(MinioClient minio, RabbitMQ.Client.IModel model)
        {
            this.minio = minio;
            this.model = model;
        }

        public void Check()
        {
            this.CheckMinioServer();
            this.CheckAMQPServer();

        }

        private void CheckMinioServer()
        {
            bool exists = minio.BucketExistsAsync(this.MinioBucketName).GetAwaiter().GetResult();
            if (exists == false)
            {
                minio.MakeBucketAsync(this.MinioBucketName).GetAwaiter();
            }
        }

        private void CheckAMQPServer()
        {
            model.QueueDeclare(this.DownloadPipelineQueue, true, false, false, null);
            model.ExchangeDeclare(this.DownloadPipelineExchange, "topic", true, false, null);
            model.QueueBind(this.DownloadPipelineQueue, this.DownloadPipelineExchange, this.DownloadPipelineRouteKey, null);
        }
    }
}
