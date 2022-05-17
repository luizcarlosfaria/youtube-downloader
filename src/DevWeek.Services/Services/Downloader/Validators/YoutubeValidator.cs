using System;
using System.Collections.Specialized;

namespace DevWeek.Services.Downloader.Validators;

public class YoutubeValidator : IUrlValidator
{
    public bool Validate(UriBuilder builder)
    {
        bool isValid = (builder.Host == "www.youtube.com");
        if (isValid)
        {
            isValid &= builder.Query != null;
            isValid &= builder.Query.Split('=').Length == 2;
            NameValueCollection queryStringParams = System.Web.HttpUtility.ParseQueryString(builder.Query);
            isValid &= queryStringParams.Count == 1;
            isValid &= queryStringParams.AllKeys[0] == "v";
        }
        return isValid;
    }
}
