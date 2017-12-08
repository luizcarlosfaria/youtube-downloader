using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Minio;

namespace DevWeekWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var outputFileName = $"/shared/{Guid.NewGuid().ToString("N")}.mp4";
            string videoUrl = "https://www.youtube.com/watch?v=qfNmyxV2Ncw";

            await DownloadMedia(videoUrl, outputFileName);
            await UploadToS3("videos", outputFileName);
            DeleteLocalFile(outputFileName);


            Console.WriteLine("Hello World!");
        }



        

        public static async Task DownloadMedia(string mediaUrl, string outputFileName)
        {
            var process = System.Diagnostics.Process.Start(new ProcessStartInfo("youtube-dl", $"-o {outputFileName} {mediaUrl}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine(await process.StandardError.ReadToEndAsync());
                Console.WriteLine(await process.StandardOutput.ReadToEndAsync());
                throw new ApplicationException("Download Failure");
            }
            string[] files = System.IO.Directory.GetFiles("/shared/", "*", SearchOption.AllDirectories);
            Console.WriteLine(string.Join(Environment.NewLine, files));
        }

        public static async Task UploadToS3(string bucketName, string fileName)
        {
            MinioClient minio = new MinioClient(
                "s3:9000",
                "Zu8VgBoZMU2xcmOEeS70",
                "xvnjYyFQyFs44iuUagi4kTHiOGvlK1PiX64LiwOy"
                );

            bool exists = await minio.BucketExistsAsync(bucketName);
            if (exists == false)
            {
                await minio.MakeBucketAsync(bucketName);
            }
            await minio.PutObjectAsync(bucketName, System.IO.Path.GetFileName(fileName), fileName);
        }

        private static void DeleteLocalFile(string outputFileName)
        {
            System.IO.File.Delete(outputFileName);
        }
    }
}
