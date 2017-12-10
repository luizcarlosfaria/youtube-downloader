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
    [Route("api/video")]
    public class VideoController : Controller
    {
        private readonly Minio.MinioClient minioClient;
        private string defaultBucketName;

        public VideoController(Minio.MinioClient minioClient, IConfiguration configuration)
        {
            this.minioClient = minioClient;
            this.defaultBucketName = configuration.GetSection("DevWeek:S3:DefaultBucketName").Get<string>();
        }

        private async Task<System.IO.MemoryStream> GetVideo(string url)
        {
            System.IO.MemoryStream streamToReturn = new System.IO.MemoryStream();
            await minioClient.GetObjectAsync(this.defaultBucketName, System.IO.Path.GetFileName(url), (stream) =>
            {
                stream.CopyTo(streamToReturn);
            });
            streamToReturn.Position = 0;
            return streamToReturn;
        }


        [HttpGet("{target}/{*address}")]
        public async Task<IActionResult> Stream(string target, string address)
        {
            
            string type = null;
            if (target == "stream")
                type = "video/mp4";
            else if (target == "download")
                type = "application/octet-stream";
            else
                return this.NotFound();

            System.IO.MemoryStream streamToReturn = await this.GetVideo(address);
            var response = File(streamToReturn, type);
            return response;
        }

    }
}