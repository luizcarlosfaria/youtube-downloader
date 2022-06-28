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

namespace DevWeek.WebApp.Controllers;

[Produces("application/json")]
//[Route("api/Enqueue")]
public class DataController : Controller
{
    private readonly IModel rabbitMQ;
    private readonly ConnectionMultiplexer redis;
    private readonly IConfiguration configuration;
    private readonly string downloadListKey;

    public DataController(IModel rabbitMQ, ConnectionMultiplexer redis, IConfiguration configuration)
    {
        this.rabbitMQ = rabbitMQ;
        this.redis = redis;
        this.configuration = configuration;
        this.downloadListKey = configuration.GetSection("DevWeek:Redis:DownloadListKey").Get<string>();
    }


    [HttpPost("api/Enqueue")]
    public void Post([FromBody] Download download)
    {
        download.Finished = null;
        download.Created = DateTime.Now;
      
        string objectInJson = System.Text.Json.JsonSerializer.Serialize(download);
        byte[] objectInByteArray = System.Text.Encoding.UTF8.GetBytes(objectInJson);

        this.rabbitMQ.BasicPublish(
            exchange: configuration.GetSection("DevWeek:RabbitMQ:DownloadPipeline:Exchange").Get<string>(),
            routingKey: configuration.GetSection("DevWeek:RabbitMQ:DownloadPipeline:RouteKey").Get<string>(),
            basicProperties: null,
            body: objectInByteArray
            );

    }

    [HttpGet("api/downloads")]
    public ActionResult Get()
    {
        //evitando desserialização desnecessária
        string[] values = redis.GetDatabase(0).ListRange(this.downloadListKey).Select(it => it.ToString()).ToArray();
        return  this.Content($"[{string.Join(",", values)}]", "application/json");
    }
}