using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevWeek.Services.Downloader.Validators;

public interface IUrlValidator
{
    bool Validate(UriBuilder builder);
}
