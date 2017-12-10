using DevWeek.Services.Downloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevWeek.Services.Downloader
{
    public class DownloadUpdateService
    {
        private readonly StackExchange.Redis.ConnectionMultiplexer redis;
        private readonly string redisDownloadListKey;

        public DownloadUpdateService(StackExchange.Redis.ConnectionMultiplexer redis, string redisDownloadListKey)
        {
            this.redis = redis;
            this.redisDownloadListKey = redisDownloadListKey;
        }


        private void WithLock(Action<StackExchange.Redis.IDatabase> action)
        {
            var redisDB = redis.GetDatabase(0);
            string lockId = Guid.NewGuid().ToString("N");

            while (redisDB.LockTake($"{this.redisDownloadListKey}-lock", lockId, TimeSpan.FromSeconds(20)) == false)
            {
                System.Threading.Thread.Sleep(100);
            }

            action(redisDB);

            redisDB.LockRelease($"{this.redisDownloadListKey}-lock", lockId);
        }

        public void Update(string mediaUrl, Action<Download> updateAction) 
        {
            WithLock((redisDB) =>
            {
                var resultList = redisDB
                  .ListRange(this.redisDownloadListKey)
                  .Select(download => Newtonsoft.Json.JsonConvert.DeserializeObject<Download>(download))
                  .ToList();

                var myItem = resultList.Single(it => it.OriginalMediaUrl.ToLowerInvariant() == mediaUrl.ToLowerInvariant());
                updateAction(myItem);
                var indexOf = resultList.IndexOf(myItem);
                redisDB.ListSetByIndex(this.redisDownloadListKey, indexOf, Newtonsoft.Json.JsonConvert.SerializeObject(myItem));
            });
        }

        public void InsertOrUpdate(Download downloadToInsert)
        {
            WithLock((redisDB) =>
            {
                var resultList = redisDB
                  .ListRange(this.redisDownloadListKey)
                  .Select(download => Newtonsoft.Json.JsonConvert.DeserializeObject<Download>(download))
                  .ToList();

                var myItem = resultList.SingleOrDefault(it => it.OriginalMediaUrl.ToLowerInvariant() == downloadToInsert.OriginalMediaUrl.ToLowerInvariant());
                if (myItem == null)
                {
                    redisDB.ListLeftPush(this.redisDownloadListKey, Newtonsoft.Json.JsonConvert.SerializeObject(downloadToInsert));
                }
                else
                {
                    var indexOf = resultList.IndexOf(myItem);
                    redisDB.ListSetByIndex(this.redisDownloadListKey, indexOf, Newtonsoft.Json.JsonConvert.SerializeObject(myItem));
                }
                
            });
        }
        
    }
}
