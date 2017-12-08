using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public class CleanupPipelineActivity : IPipelineActivity
    {
        public Task ExecuteAsync(Dictionary<string, string> context)
        {
            string fileName = context["outputFilePath"];

            System.IO.File.Delete(fileName);

            return Task.CompletedTask;
        }
    }
}
