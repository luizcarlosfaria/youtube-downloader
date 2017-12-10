using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Services.Downloader
{
    public class DownloadContext : Dictionary<string, object>
    {
        public string MediaUrl { get { return (string)this["mediaUrl"]; } set { this["mediaUrl"] = value; } }
        public string OutputFileName { get { return (string)this["outputFileName"]; } set { this["outputFileName"] = value; } }
        public string OutputFilePath { get { return (string)this["outputFilePath"]; } set { this["outputFilePath"] = value; } }
        public Download Download { get { return (Download)this["downloadObject"]; } set { this["downloadObject"] = value; } }
    }
}
