using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek.Services.Downloader
{
    public class DownloadContext
    {
        const String LocalTemporaryFolder = "//shared//";

        public string VideoOutputFilePath { get; set; }
        public string AudioOutputFilePath { get; set; }

        public Download Download { get; set; }
    }
}
