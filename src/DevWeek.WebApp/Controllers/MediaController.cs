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
    [Route("api/media")]
    public class MediaController : Controller
    {
        private readonly Minio.MinioClient minioClient;

        public MediaController(Minio.MinioClient minioClient, IConfiguration configuration)
        {
            this.minioClient = minioClient;
        }

        private async Task<System.IO.MemoryStream> GetMedia(string bucket, string url)
        {
            System.IO.MemoryStream streamToReturn = new System.IO.MemoryStream();
            await minioClient.GetObjectAsync(bucket, System.IO.Path.GetFileName(url), (stream) =>
            {
                stream.CopyTo(streamToReturn);
            });
            streamToReturn.Position = 0;
            return streamToReturn;
        }


        [HttpGet("{bucket}/{target}/{*address}")]
        public async Task<IActionResult> GetMedia(string target, string bucket, string address)
        {
            this.Response.Headers.Add("Accept-Ranges", "bytes"); 
        

            string type = null;
            if (target == "stream")
                type = "video/mp4";
            else if (target == "download")
                type = "application/octet-stream";
            else
                return this.NotFound();

            System.IO.MemoryStream streamToReturn = await this.GetMedia(bucket, address);
            var response = File(streamToReturn, type);
            return response;
        }

    }
}