using DevWeek.Services.Downloader.Validators;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public class EntryPointRegisterPipelineActivity : IPipelineActivity
    {

        private readonly DataService metadataUpdater;
        public IList<IUrlValidator> Validators { get; set; }

        public EntryPointRegisterPipelineActivity(DataService metadataUpdater)
        {
            this.metadataUpdater = metadataUpdater;
        }


        public async Task ExecuteAsync(Download download)
        {
            if (download.OriginalMediaUrl == null)
                throw new ArgumentNullException($"The url '{download.OriginalMediaUrl}' is null #invalidUrl");
            
            this.ValidateUrl(download.OriginalMediaUrl);

            await this.metadataUpdater.Insert(download);

        }

        private void ValidateUrl(string url)
        {
            UriBuilder builder = null;
            try
            {
                builder = new UriBuilder(url);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The url '{url}' is not valid #invalidUrl", ex);
            }


            if (this.Validators.Any(it => it.Validate(builder)) == false)
            {
                throw new InvalidOperationException($"The url '{url}' is not valid #invalidUrl");
            }

        }



       

    }
}