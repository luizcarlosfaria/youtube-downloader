using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DevWeek.Services.Downloader.MediaDownloader
{
    public class GenericOutputMetadataExtractor: IMetadataExtractor
    {
        public string StartsWith { get; set; }

        public string EndsWith { get; set; }

        public Regex Regex { get; set; }

        public string Extract(string[] outputLines)
        {
            var line = outputLines.Where(it => 
                (this.StartsWith == null || it.StartsWith(this.StartsWith))
                &&
                (this.EndsWith == null || it.EndsWith(this.EndsWith))

            ).SingleOrDefault();

            if (line != null)
            {
                if (this.Regex != null)
                    return this.MatchUsingRegex(line);
                else
                    return this.MatchUnderStartAndEnd(line);
            }

            return null;
        }

        private string MatchUsingRegex(string line)
        {
            var matchCollection = this.Regex.Matches(line);
            if (matchCollection.Count == 1)
            {
                return matchCollection[0].Value.Unquote();
            }
            return null;
        }

        private string MatchUnderStartAndEnd(string line)
        {
            int startLength = this.StartsWith?.Length ?? 0;
            int endLength = this.EndsWith?.Length ?? 0;

            return line.Substring(startLength, line.Length - (startLength + endLength));
        }
    }

    public static class QuoteExtensions
    {
        public static string Unquote(this string text)
        {
            text = text.Trim();
            if ((text.StartsWith("\"") || text.StartsWith("'")) && (text.EndsWith("\"") || text.EndsWith("'")))
                return text.Substring(1, text.Length - 2);
            return text;
        }
    }
}
