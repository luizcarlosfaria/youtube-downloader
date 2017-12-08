using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Minio;

namespace DevWeek
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var appContext = ContextBuilder.BuildContext();

            var pipeline = appContext.GetObject<DevWeek.Services.Downloader.DownloadPipeline>();
            await pipeline.Run(new Dictionary<string, string>() { 
                { "mediaUrl", "https://www.youtube.com/watch?v=qfNmyxV2Ncw" } ,
                { "outputFileName", $"{Guid.NewGuid().ToString("N")}.mp4" }
            });

            //var outputFileName = $"/shared/{Guid.NewGuid().ToString("N")}.mp4";
            //string videoUrl = "https://www.youtube.com/watch?v=qfNmyxV2Ncw";


            //await MediaDownlader.UploadToS3("videos", outputFileName);
            //MediaDownlader.DeleteLocalFile(outputFileName);

            await Task.CompletedTask;
            Console.WriteLine("Hello World!");
        }



        

    }
}
