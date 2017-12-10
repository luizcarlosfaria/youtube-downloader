using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public class MediaDownloaderPipelineActivity : IPipelineActivity
    {
        public async Task ExecuteAsync(DownloadContext context)
        {
            var process = System.Diagnostics.Process.Start(new ProcessStartInfo("youtube-dl", $"-o {context.OutputFilePath} {context.MediaUrl}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                //Console.WriteLine(await process.StandardError.ReadToEndAsync());
                //Console.WriteLine(await process.StandardOutput.ReadToEndAsync());
                throw new ApplicationException("Download Failure", new Exception(await process.StandardError.ReadToEndAsync() + await process.StandardOutput.ReadToEndAsync()));
            }
        }
    }
}
