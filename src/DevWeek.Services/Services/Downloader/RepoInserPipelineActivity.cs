using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public class RepoInserPipelineActivity : IPipelineActivity
    {

        private readonly DownloadUpdateService metadataUpdater;

        public RepoInserPipelineActivity(DownloadUpdateService metadataUpdater)
        {
            this.metadataUpdater = metadataUpdater;
        }


        public Task ExecuteAsync(DownloadContext context)
        {
            if (context.Download.OriginalMediaUrl == null)
                throw new InvalidOperationException($"The url '{context.Download.OriginalMediaUrl}' is not valid #invalidUrl");

            UriBuilder builder = null;
            try
            {
                builder = new UriBuilder(context.Download.OriginalMediaUrl.ToLowerInvariant());
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"The url '{context.Download.OriginalMediaUrl}' is not valid #invalidUrl");
            }

            if((builder.Host != "www.youtube.com" && builder.Host != "youtu.be") || builder.Query.ToLowerInvariant().Contains("list="))
                throw new InvalidOperationException($"The url '{context.Download.OriginalMediaUrl}' is not valid #invalidUrl");


            metadataUpdater.InsertOrUpdate(context.Download);

            return Task.CompletedTask;
        }
    }
}