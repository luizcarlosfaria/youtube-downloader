using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Services.Downloader
{
    public class DownloadContext
    {
        const String LocalTemporaryFolder = "//shared//";


        public string VideoOutputFileName => $"{this.Download.Id}.mp4";
        public string AudioOutputFileName => $"{this.Download.Id}.mp3";

        public string VideoOutputFilePath => $"{System.IO.Path.Combine(LocalTemporaryFolder, this.VideoOutputFileName)}";
        public string AudioOutputFilePath => $"{System.IO.Path.Combine(LocalTemporaryFolder , this.AudioOutputFileName)}";

        public Download Download { get; set; }
    }
}
