using Minio;
using System.IO;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader;

public abstract class MediaBaseActivity
{
    private readonly MinioClient minio;

    public string SharedPath { get; set; }

    protected MediaBaseActivity(MinioClient minio)
    {
        this.minio = minio;
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

    protected Task UploadToS3Async(MinioObject minioObject, string filePath)
       => this.minio.PutObjectAsync(minioObject.BucketName, minioObject.ObjectName, filePath);

    protected Task DownloadFromS3Async(MinioObject minioObject, string filePath)
       => this.minio.GetObjectAsync(minioObject.BucketName, minioObject.ObjectName, filePath);

}



