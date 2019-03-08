using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DevWeek.Services.Downloader.MediaDownloader;

namespace DevWeek.Services.Downloader
{
    public class MediaDownloaderPipelineActivity : IPipelineActivity
    {
        public string SharedPath { get; set; }

        public List<IMetadataExtractor> MetadataExtractors { get; set; }

        public Task ExecuteAsync(DownloadContext context)
        {
            VideoDownload(context);

            AudioDownload(context);

            return Task.CompletedTask;
        }

        private void VideoDownload(DownloadContext context)
        {
            string defaultDownloadPath = Path.Combine(this.SharedPath, $"{context.Download.Id}.mp4");

            var processStartInfo = new ProcessStartInfo("youtube-dl", $"-o {defaultDownloadPath} {context.Download.OriginalMediaUrl}");

            (string standardOutput, _) = this.Run(processStartInfo);

            context.VideoOutputFilePath = this.ExtractPath(defaultDownloadPath, standardOutput);
        }

        private void AudioDownload(DownloadContext context)
        {
            string defaultDownloadPath = Path.Combine(this.SharedPath, $"{context.Download.Id}.mp3");

            var processStartInfo = new ProcessStartInfo("ffmpeg", $"-i {context.VideoOutputFilePath} {defaultDownloadPath}");

            (string standardOutput, _) = this.Run(processStartInfo);

            context.AudioOutputFilePath = this.ExtractPath(defaultDownloadPath, standardOutput);
        }

        private string ExtractPath(string defaultPath, string processExecutionOutput)
        {
            string returnValue = null;

            if (File.Exists(defaultPath))
            {
                returnValue = defaultPath;
            }
            else
            {
                var lines = processExecutionOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var metadataExtractor in this.MetadataExtractors)
                {
                    string extractResult = metadataExtractor.Extract(lines);

                    if (extractResult != null && File.Exists(extractResult))
                    {
                        returnValue = extractResult;
                        break;
                    }
                }
            }

            if (returnValue == null || !File.Exists(returnValue))
                throw new ApplicationException("Could not found media file", new Exception("Process Output: " + processExecutionOutput));

            return returnValue;
        }


        private (string output, string error) Run(ProcessStartInfo processStartInfo)
        {
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;

            var process = Process.Start(processStartInfo);

            process.WaitForExit();

            string standardError = process.StandardError.ReadToEndAsync().GetAwaiter().GetResult();
            string standardOutput = process.StandardOutput.ReadToEndAsync().GetAwaiter().GetResult();

            if (process.ExitCode != 0)
            {
                throw new ApplicationException("Download Failure", new Exception(standardError + Environment.NewLine + standardOutput));
            }

            return (standardOutput, standardError);
        }
    }

}
