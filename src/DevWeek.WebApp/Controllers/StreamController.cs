using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DevWeek.WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/stream")]
    public class StreamController : Controller
    {
        private readonly Minio.MinioClient minioClient;
        private string defaultBucketName;

        public StreamController(Minio.MinioClient minioClient, IConfiguration configuration) {
            this.minioClient = minioClient;
            this.defaultBucketName = configuration.GetSection("DevWeek:S3:DefaultBucketName").Get<string>();
        }

        [HttpGet("{*url}")]
        public async Task<IActionResult> Get(string url)
        {

            System.IO.MemoryStream streamToReturn = new System.IO.MemoryStream();
            await minioClient.GetObjectAsync(this.defaultBucketName, System.IO.Path.GetFileName(url), (stream) => {
                stream.CopyTo(streamToReturn);
            });
            streamToReturn.Position = 0;
            var response = File(streamToReturn, "video/mp4"); // FileStreamResult
            return response;
        }
    }
}