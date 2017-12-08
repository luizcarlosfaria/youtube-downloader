using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DevWeek.Services.Downloader
{
    public class PathCombinePipelineActivity : IPipelineActivity
    {
        public string[] Keys { get; set; }

        public string OutputKey { get; set; }

        public Task ExecuteAsync(Dictionary<string, string> context)
        {
            string[] contextValues = this.Keys.Select(key => context[key]).ToArray();

            context[this.OutputKey] = System.IO.Path.Combine(contextValues);

            return Task.CompletedTask;
        }
    }
}
