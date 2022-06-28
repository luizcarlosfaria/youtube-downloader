using System;

namespace DevWeek.Services.Downloader.Validators;

public interface IUrlValidator
{
    bool Validate(UriBuilder builder);
}
