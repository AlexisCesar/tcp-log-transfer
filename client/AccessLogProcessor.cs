using System.Text.RegularExpressions;

namespace client
{
    internal class AccessLogProcessor
    {
        private readonly Regex logPattern = new Regex("^(?<client>\\S+) \\S+ (?<userid>\\S+) \\[(?<datetime>[^\\]]+)\\] \"(?<method>\\S+)\\s?(?<request>\\S+)?\\s?(\\S+)? (?<status>[0-9]{3}) (?<size>[0-9]+|-)", RegexOptions.Compiled);
        public void processLogLine(string line)
        {
            throw new NotImplementedException();
        }

        public AccessLogRegister? processLogLineAndReturnIt(string line)
        {
            Match match = logPattern.Match(line);

            return new AccessLogRegister()
            {
                SourceIPAddress = match.Groups["client"].Value,

                User = match.Groups["userid"].Value,

                StatusCode = (match.Groups["status"].Value == null || match.Groups["status"].Value == "-") ? null : int.Parse(match.Groups["status"].Value),

                RequestBytes = (match.Groups["size"].Value == null || match.Groups["size"].Value == "-") ? null : int.Parse(match.Groups["size"].Value),

                Url = match.Groups["request"].Value,

                BrazilianTime = String.IsNullOrEmpty(match.Groups["datetime"].Value) ? null : DateTime.Parse(match.Groups["datetime"].Value.Substring(0, 11) + ' ' + match.Groups["datetime"].Value.Substring(12))
            };
        }
    }
}
