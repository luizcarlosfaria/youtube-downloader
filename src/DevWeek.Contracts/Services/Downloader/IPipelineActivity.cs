using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader
{
    public interface IPipelineActivity
    {
        Task ExecuteAsync(DownloadContext context);
    }
}
