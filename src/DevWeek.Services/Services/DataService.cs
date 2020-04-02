using DevWeek.Services.Downloader;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services
{
    /// <summary>
    /// Manage Download Data
    /// </summary>
    public class DataService
    {
        private readonly StackExchange.Redis.ConnectionMultiplexer redis;
        private readonly DistributedLockService distributedLockService;
        private readonly MongoClient mongoClient;
        private readonly string redisDownloadListKey;
        private readonly string mongoDownloadCollectionName;

        public DataService(StackExchange.Redis.ConnectionMultiplexer redis, DistributedLockService distributedLockService, MongoClient mongoClient, string redisDownloadListKey, string mongoDownloadCollectionName)
        {
            this.redis = redis;
            this.distributedLockService = distributedLockService;
            this.mongoClient = mongoClient;
            this.redisDownloadListKey = redisDownloadListKey;
            this.mongoDownloadCollectionName = mongoDownloadCollectionName;
        }

        public async Task Update(string id, Func<UpdateDefinitionBuilder<Download>, UpdateDefinition<Download>> updateFunction)
        {
            var downloadItemOnDB = this.GetDownloadElementById(id);
            if (downloadItemOnDB != null)
            {
                var downloadCollection = this.GetDownloadCollection();
                var update = updateFunction(Builders<Download>.Update);
                downloadCollection.FindOneAndUpdate(it => it.Id == id, update);
                this.RebuildCache(downloadCollection);
            }
            else
            {
                throw new InvalidOperationException($"Download with id '{id}' can not found! #invalidId");
            }
        }

        public async Task Insert(Download downloadToInsert)
        {
            var downloadItemOnDB = this.GetDownloadElementByUrl(downloadToInsert.OriginalMediaUrl);
            if (downloadItemOnDB == null)
            {
                var downloadCollection = this.GetDownloadCollection();
                await downloadCollection.InsertOneAsync(downloadToInsert);
                this.RebuildCache(downloadCollection);
            }
            else
            {
                downloadToInsert.Id = downloadItemOnDB.Id;
            }
        }

        public void RebuildCache(IMongoCollection<Download> downloadCollection = null)
        {
            downloadCollection = downloadCollection ?? this.GetDownloadCollection();

            using (this.distributedLockService.Acquire(0, $"{this.redisDownloadListKey}-key", TimeSpan.FromSeconds(15)))
            {
                var mongoList = downloadCollection.Find(Builders<Download>.Filter.Empty).ToList();

                var list = mongoList.ConvertAll(it => { return (RedisValue)Newtonsoft.Json.JsonConvert.SerializeObject(it); });

                IDatabase redisDB = this.redis.GetDatabase(0);

                redisDB.KeyDelete(this.redisDownloadListKey);

                redisDB.ListLeftPush(this.redisDownloadListKey, list.ToArray());
            };
        }


        private IMongoCollection<Download> GetDownloadCollection()
        {
            var downloadCollection = this.mongoClient.GetDatabase("admin").GetCollection<Download>(this.mongoDownloadCollectionName);
            return downloadCollection;
        }

        private Download GetDownloadElementByUrl(string url)
        {
            var downloadItemOnDB = this.GetDownloadCollection().AsQueryable().SingleOrDefault(it => it.OriginalMediaUrl.ToLowerInvariant() == url.ToLowerInvariant());
            return downloadItemOnDB;
        }

        private Download GetDownloadElementById(string id)
        {
            var downloadItemOnDB = this.GetDownloadCollection().AsQueryable().SingleOrDefault(it => it.Id == id);
            return downloadItemOnDB;
        }
    }

}

