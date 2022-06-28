namespace DevWeek.Services.Downloader;

public interface IPipelineActivity
{
    Task ExecuteAsync(Download context);
}
