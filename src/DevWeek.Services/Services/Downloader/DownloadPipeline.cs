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

        public Dictionary<string,object> Context { get; set; }

        public async Task Run(Dictionary<string, object> contextOverrides = null)
        {
            DownloadContext currentContext = this.BuildContext(this.Context, contextOverrides);
            
            foreach (IPipelineActivity pipelineActivity in this.Activities)
            {
                await pipelineActivity.ExecuteAsync(currentContext);
            }

        }

        private DownloadContext BuildContext(Dictionary<string, object> context, Dictionary<string, object> contextOverrides)
        {
            var currentContext = new DownloadContext();

            Action<KeyValuePair<string, object>> addItemTtoContext = (KeyValuePair<string, object> item) =>
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
