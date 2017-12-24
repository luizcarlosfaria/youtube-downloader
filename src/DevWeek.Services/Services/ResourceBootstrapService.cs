using Minio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Core;

namespace DevWeek.Services
{
    /// <summary>
    /// Initialize infrastructure without lock
    /// </summary>
    public class ResourceBootstrapService
    {
        private readonly MinioClient minio;
        private readonly MongoClient mongoClient;
        private readonly DistributedLockService distributedLockService;
        private readonly RabbitMQ.Client.IModel model;

        public string[] MongoRequiredCollections { get; set; }
        public string[] MinioBucketNames { get; set; }
        public string DownloadPipelineQueue { get; set; }
        public string DownloadPipelineRouteKey { get; set; }
        public string DownloadPipelineExchange { get; set; }

        public TimeSpan LockTimeout { get; set; }
        public string DistributedLockKey { get; set; }

        /// <summary>
        /// Initialize a new instance of ResourceBootstrapService passing required connectors
        /// </summary>
        /// <param name="distributedLockService">DistributedLockService</param>
        /// <param name="minio">S3</param>
        /// <param name="mongoClient">MongoDB</param>
        /// <param name="model">RabbitMQ</param>
        public ResourceBootstrapService(DistributedLockService distributedLockService, MinioClient minio, MongoClient mongoClient, RabbitMQ.Client.IModel model)
        {
            this.minio = minio;
            this.mongoClient = mongoClient;
            this.distributedLockService = distributedLockService;
            this.model = model;
        }

        /// <summary>
        /// Verify and initialize infrastructure
        /// </summary>
        public void Check()
        {
            using (this.distributedLockService.Acquire(0, this.DistributedLockKey, this.LockTimeout))
            {
                this.CheckMinioServer();
                this.CheckAMQPServer();
                this.CkeckMongoDB();
            }
        }

        /// <summary>
        /// Perform a check on S3 and create a bucket if necessary 
        /// </summary>
        private void CheckMinioServer()
        {
            foreach (string minioBucketName in this.MinioBucketNames)
            {
                bool exists = minio.BucketExistsAsync(minioBucketName).GetAwaiter().GetResult();
                if (exists == false)
                {
                    minio.MakeBucketAsync(minioBucketName).GetAwaiter();
                }
            }
        }


        /// <summary>
        /// Perform a check on MongoDB and create some collections if necessary 
        /// </summary>
        private void CkeckMongoDB()
        {
            var database = this.mongoClient.GetDatabase("admin");
            var collectionNames = database.ListCollections().ToList().Select(it => it["name"].AsString).ToArray();
            var collectionNamesForCreation = this.MongoRequiredCollections.Where(it => collectionNames.Contains(it) == false).ToArray();
            foreach (var collectionName in collectionNamesForCreation)
            {
                database.CreateCollection(collectionName);
            }
        }


        /// <summary>
        /// Check RabbitMQ virtual host overiding Queue, Exchange and Bind between Exchange and Queue
        /// </summary>
        private void CheckAMQPServer()
        {
            model.QueueDeclare(this.DownloadPipelineQueue, true, false, false, null);
            model.ExchangeDeclare(this.DownloadPipelineExchange, "topic", true, false, null);
            model.QueueBind(this.DownloadPipelineQueue, this.DownloadPipelineExchange, this.DownloadPipelineRouteKey, null);
        }
    }
}
