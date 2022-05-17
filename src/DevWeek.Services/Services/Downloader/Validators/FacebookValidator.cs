using System;

namespace DevWeek.Services.Downloader.Validators;

public class FacebookValidator : IUrlValidator
{
    public bool Validate(UriBuilder builder)
    {
        bool isValid = (builder.Host == "www.facebook.com");

        if (isValid)
        {
            isValid &= string.IsNullOrWhiteSpace(builder.Path) == false;
            string[] pathParts = builder.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            isValid &= pathParts.Length == 3;
            isValid &= pathParts[1] == "videos";
        }
        return isValid;
    }
}
