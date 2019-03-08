using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Services.Downloader.MediaDownloader
{
    public interface IMetadataExtractor
    {
        string Extract(string[] outputLines);
    }
}
