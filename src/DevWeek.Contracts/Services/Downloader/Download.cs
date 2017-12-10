using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Services.Downloader
{
    public class Download
    {
        public string OriginalMediaUrl { get; set; }

        public string MinioAddress { get; set; }
        
        public DateTime Created { get; set; }

        public DateTime? Finished { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public string DownloadUrl { get; set; }
    }
}
