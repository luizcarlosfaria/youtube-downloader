using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DevWeek.Services.Downloader
{
    public class DownloadPipeline
    {
        public List<IPipelineActivity> Activities { get; set; }

        public Dictionary<string, string> Context { get; set; }

        public async Task Run(Dictionary<string, string> contextOverrides = null)
        {
            Dictionary<string, string> currentContext = this.BuildContext(this.Context, contextOverrides);
            
            foreach (IPipelineActivity pipelineActivity in this.Activities)
            {
                await pipelineActivity.ExecuteAsync(currentContext);
            }

        }

        private Dictionary<string, string> BuildContext(Dictionary<string, string> context, Dictionary<string, string> contextOverrides)
        {
            var currentContext = new Dictionary<string, string>();

            Action<KeyValuePair<string, string>> addItemTtoContext = (KeyValuePair<string, string> item) =>
            {
                if (currentContext.ContainsKey(item.Key))
                    currentContext[item.Key] = item.Value;
                else
                    currentContext.Add(item.Key, item.Value);
            };

            foreach (var item in context) { addItemTtoContext(item); }
            foreach (var item in contextOverrides) { addItemTtoContext(item); }

            return currentContext;
        }
    }
}
