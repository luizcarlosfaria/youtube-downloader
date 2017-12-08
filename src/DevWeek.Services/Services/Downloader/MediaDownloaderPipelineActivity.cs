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
        public async Task ExecuteAsync(Dictionary<string, string> context)
        {
            string outputFilePath = context["outputFilePath"];
            string mediaUrl = context["mediaUrl"];


            var process = System.Diagnostics.Process.Start(new ProcessStartInfo("youtube-dl", $"-o {outputFilePath} {mediaUrl}")
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
        }
    }
}
