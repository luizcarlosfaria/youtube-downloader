using Minio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek
{
    public class MediaDownlader
    {

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
