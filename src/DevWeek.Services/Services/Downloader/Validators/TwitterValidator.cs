using System;

namespace DevWeek.Services.Downloader.Validators;

public class TwitterValidator : IUrlValidator
{
    public bool Validate(UriBuilder builder)
    {
        bool isValid = (builder.Host == "twitter.com");
        if (isValid)
        {
            isValid &= string.IsNullOrWhiteSpace(builder.Path) == false;
        }
        return isValid;
    }
}
