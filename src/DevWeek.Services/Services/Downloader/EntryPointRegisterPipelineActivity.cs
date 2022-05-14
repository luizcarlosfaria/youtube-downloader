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

        public EntryPointRegisterPipelineActivity(DataService metadataUpdater)
        {
            this.metadataUpdater = metadataUpdater;
        }


        public async Task ExecuteAsync(DownloadContext context)
        {
            if (context.Download.OriginalMediaUrl == null)
                throw new ArgumentNullException($"The url '{context.Download.OriginalMediaUrl}' is null #invalidUrl");
            
            this.ValidateUrl(context.Download.OriginalMediaUrl);

            await this.metadataUpdater.Insert(context.Download);

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

            switch (builder.Host)
            {
                case "www.youtube.com":
                    this.ValidateYoutubeLongUrl(builder);
                    break;
                case "youtu.be":
                    this.ValidateYoutubeShort(builder);
                    break;
                case "www.facebook.com":
                    this.ValidateFacebookUrl(builder);
                    break;
                case "twitter.com":
                    this.ValidateTwitterUrl(builder);
                    break;
                default:
                    throw new InvalidOperationException($"The url '{url}' is not valid #invalidUrl");
            }
        }



        /// <summary>
        /// Validate a url using rules for short youtube urls
        /// </summary>
        /// <remarks>
        ///     Example: https://youtu.be/AxRVUNtvbcc
        /// </remarks>
        /// <param name="builder"></param>
        private void ValidateYoutubeShort(UriBuilder builder)
        {
            bool isValid = (builder.Host == "youtu.be");
            isValid &= string.IsNullOrWhiteSpace(builder.Query);
            isValid &= string.IsNullOrWhiteSpace(builder.Path) == false;
            isValid &= builder.Path.Split('/').Length == 2;

            if (isValid == false)
            {
                throw new InvalidOperationException($"The url '{builder.ToString()}' is not valid #invalidUrl");
            }
        }


        /// <summary>
        /// Validate a url using rules for short youtube urls
        /// </summary>
        /// <remarks>
        ///     Example: https://youtu.be/AxRVUNtvbcc
        /// </remarks>
        /// <param name="builder"></param>
        private void ValidateFacebookUrl(UriBuilder builder)
        {
            bool isValid = (builder.Host == "www.facebook.com");
            isValid &= string.IsNullOrWhiteSpace(builder.Path) == false;
            string[] pathParts = builder.Path.Split(new char[] { '/' },  StringSplitOptions.RemoveEmptyEntries);
            isValid &= pathParts.Length == 3;
            isValid &= pathParts[1] == "videos";



            if (isValid == false)
            {
                throw new InvalidOperationException($"The url '{builder.ToString()}' is not valid #invalidUrl");
            }
        }

        /// <summary>
        /// Validate a url using rules for twitter urls
        /// </summary>
        /// <remarks>
        ///     Example: https://youtu.be/AxRVUNtvbcc
        /// </remarks>
        /// <param name="builder"></param>
        private void ValidateTwitterUrl(UriBuilder builder)
        {
            bool isValid = (builder.Host == "twitter.com");
            isValid &= string.IsNullOrWhiteSpace(builder.Path) == false;
            //isValid = builder.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Contains("video");

            if (isValid == false)
            {
                throw new InvalidOperationException($"The url '{builder.ToString()}' is not valid #invalidUrl");
            }
        }

        /// <summary>
        /// Validate a url using rules for long youtube urls
        /// </summary>
        /// <remarks>
        ///     Example: https://www.youtube.com/watch?v=SQJIDvirfp4
        /// </remarks>
        /// <param name="builder"></param>
        private void ValidateYoutubeLongUrl(UriBuilder builder)
        {
            bool isValid = (builder.Host == "www.youtube.com");
            isValid &= builder.Query != null;
            isValid &= builder.Query.Split('=').Length == 2;
            NameValueCollection queryStringParams = System.Web.HttpUtility.ParseQueryString(builder.Query);
            isValid &= queryStringParams.Count == 1;
            isValid &= queryStringParams.AllKeys[0] == "v";
            if (isValid == false)
            {
                throw new InvalidOperationException($"The url '{builder.ToString()}' is not valid #invalidUrl");
            }
        }
    }
}