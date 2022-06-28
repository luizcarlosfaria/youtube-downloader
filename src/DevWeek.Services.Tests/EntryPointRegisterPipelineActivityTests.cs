using DevWeek.Services.Downloader;
using System;
using Xunit;

namespace DevWeek.Services.Tests;

public class EntryPointRegisterPipelineActivityTests
{
    private Download BuildContext(string url)
    {
        return new Download()
        {
            OriginalMediaUrl = url
        };
    }


    [Fact]
    public void GenericGoogleAddress()
    {
        IPipelineActivity activity = new EntryPointRegisterPipelineActivity(null);
        Download download = this.BuildContext("https://www.google.com");

        Assert.Throws<InvalidOperationException>(() =>
        {
            activity.ExecuteAsync(download).GetAwaiter().GetResult();
        });
    }

    [Fact]
    public void NullAddress()
    {
        IPipelineActivity activity = new EntryPointRegisterPipelineActivity(null);
        Download download = this.BuildContext(null);
        Assert.Throws<ArgumentNullException>(() =>
        {
            activity.ExecuteAsync(download).GetAwaiter().GetResult();
        });
    }
    [Fact]
    public void Playlist()
    {
        IPipelineActivity activity = new EntryPointRegisterPipelineActivity(null);
        Download download = this.BuildContext("https://www.youtube.com/watch?v=gFTE__qOMqI&t=306s&index=1&list=PLbDr6zG2yjZPD-FvY6cBQQAB8j7tjXD11");
        Assert.Throws<InvalidOperationException>(() =>
        {
            activity.ExecuteAsync(download).GetAwaiter().GetResult();
        });
    }

    [Fact]
    public void OkLongUrl()
    {
        IPipelineActivity activity = new EntryPointRegisterPipelineActivity(null);
        Download download = this.BuildContext("https://www.youtube.com/watch?v=PH0SqgTSocs");
        Assert.Throws<NullReferenceException>(() =>
        {
            activity.ExecuteAsync(download).GetAwaiter().GetResult();
        });
    }


    [Fact]
    public void OkShortUrl()
    {
        IPipelineActivity activity = new EntryPointRegisterPipelineActivity(null);
        Download download = this.BuildContext("https://youtu.be/PH0SqgTSocs");
        Assert.Throws<NullReferenceException>(() =>
        {
            activity.ExecuteAsync(download).GetAwaiter().GetResult();
        });
    }
}
