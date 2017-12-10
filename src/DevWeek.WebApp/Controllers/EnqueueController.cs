using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevWeek.Services.Downloader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace DevWeek.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Enqueue")]
    public class EnqueueController : Controller
    {
        private readonly IModel rabbitMQ;
        private readonly ConnectionMultiplexer redis;
        private readonly IConfiguration configuration;
        private readonly string downloadListKey;

        public EnqueueController(IModel rabbitMQ, ConnectionMultiplexer redis, IConfiguration configuration)
        {
            this.rabbitMQ = rabbitMQ;
            this.redis = redis;
            this.configuration = configuration;
            this.downloadListKey = configuration.GetSection("DevWeek:Redis:DownloadListKey").Get<string>();
        }


        [HttpPost]
        public void Post([FromBody] Download download)
        {
            download.MinioAddress = null;
            download.Finished = null;
            download.Created = DateTime.Now;

            string objectInJson = Newtonsoft.Json.JsonConvert.SerializeObject(download);
            byte[] objectInByteArray = System.Text.Encoding.UTF8.GetBytes(objectInJson);

            this.rabbitMQ.BasicPublish(
                exchange: configuration.GetSection("DevWeek:RabbitMQ:DownloadPipeline:Exchange").Get<string>(),
                routingKey: configuration.GetSection("DevWeek:RabbitMQ:DownloadPipeline:RouteKey").Get<string>(),
                null,
                objectInByteArray
                );

        }

        [HttpGet]
        public IEnumerable<Download> Get()
        {
            var result = redis.GetDatabase(0)
                .ListRange(this.downloadListKey)
                .Select(download =>
                {
                    var instance = Newtonsoft.Json.JsonConvert.DeserializeObject<Download>(download);
                    if (string.IsNullOrWhiteSpace(instance.DownloadUrl) == false)
                    {
                        UriBuilder uriBuilder = new UriBuilder(instance.DownloadUrl);
                        uriBuilder.Host = this.Request.Host.Host;
                        uriBuilder.Port = this.Request.Host.Port ?? 80;
                        uriBuilder.Path = uriBuilder.Path.Insert(0, "/api/stream");
                        instance.DownloadUrl = uriBuilder.ToString();
                    }
                    return instance;
                })
                .ToArray();
            return result;
        }
    }
}