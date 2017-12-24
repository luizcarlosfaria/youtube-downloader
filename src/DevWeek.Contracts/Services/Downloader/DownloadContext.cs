using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Services.Downloader
{
    public class DownloadContext : Dictionary<string, object>
    {
        public string MediaUrl { get { return (string)this["mediaUrl"]; } set { this["mediaUrl"] = value; } }


        public string VideoOutputFileName => $"{this.Download.Id}.mp4";
        public string AudioOutputFileName => $"{this.Download.Id}.mp3";

        public string VideoOutputFilePath => $"{System.IO.Path.Combine((string)this["localTemporaryFolder"], this.VideoOutputFileName)}";
        public string AudioOutputFilePath => $"{System.IO.Path.Combine((string)this["localTemporaryFolder"], this.AudioOutputFileName)}";

        public Download Download { get { return (Download)this["downloadObject"]; } set { this["downloadObject"] = value; } }
    }
}
