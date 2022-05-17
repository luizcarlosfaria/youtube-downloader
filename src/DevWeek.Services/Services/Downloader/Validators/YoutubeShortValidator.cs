using System;

namespace DevWeek.Services.Downloader.Validators;

public class YoutubeShortValidator : IUrlValidator
{
    public bool Validate(UriBuilder builder)
    {
        bool isValid = (builder.Host == "youtu.be");
        if (isValid)
        {
            isValid &= string.IsNullOrWhiteSpace(builder.Query);
            isValid &= string.IsNullOrWhiteSpace(builder.Path) == false;
            isValid &= builder.Path.Split('/').Length == 2;
        }
        return isValid;
    }
}
