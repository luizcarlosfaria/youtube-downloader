using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Minio;

namespace DevWeek.Services.Downloader
{
    public abstract class MediaBaseActivity
    {
        private readonly MinioClient minio;

        public string SharedPath { get; set; }

        protected MediaBaseActivity(MinioClient minio)
        {
            this.minio = minio;
        }

        protected (string output, string error, int exitCode) Run(ProcessStartInfo processStartInfo, bool throwExceptionOnFalure = true)
        {
            (string standardOutput, string standardError, int exitCode) = this.PermissiveRun(processStartInfo);

            if (exitCode != 0 && throwExceptionOnFalure)
            {
                throw new ApplicationException("Download Failure", new Exception(standardError + Environment.NewLine + standardOutput));
            }

            return (standardOutput, standardError, exitCode);
        }

        protected (string output, string error, int exitCode) PermissiveRun(ProcessStartInfo processStartInfo)
        {
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;

            var process = Process.Start(processStartInfo);

            process.WaitForExit();

            string standardError = process.StandardError.ReadToEndAsync().GetAwaiter().GetResult();
            string standardOutput = process.StandardOutput.ReadToEndAsync().GetAwaiter().GetResult();

            return (standardOutput, standardError, process.ExitCode);
        }

        protected string TryCreateDownloadDirectory(Download download)
        {
            string newDirectory = Path.Combine(this.SharedPath, download.Id);

            if (Directory.Exists(newDirectory) == false)
            {
                System.IO.Directory.CreateDirectory(newDirectory);
            }

            return newDirectory;
        }

        protected Task UploadToS3(MinioObject minioObject, string filePath)
           => this.minio.PutObjectAsync(minioObject.BucketName, minioObject.ObjectName, filePath);

        protected Task DownloadFromS3(MinioObject minioObject, string filePath)
           => this.minio.GetObjectAsync(minioObject.BucketName, minioObject.ObjectName, filePath);

    }



}
