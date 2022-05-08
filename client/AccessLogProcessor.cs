using System.Net;

namespace client
{
    internal class AccessLogProcessor : ILogProcessor<AccessLogRegister>
    {
        public void processLogLine(string line)
        {
            Console.WriteLine(line);
        }

        public AccessLogRegister processLogLineAndReturnIt(string line)
        {
            var content = line.Split(' ');

            return new AccessLogRegister()
            {
                SourceIPAddress = content[0],
                StatusCode = int.Parse(content[8]),
                RequestBytes = int.Parse(content[9]),
                Url = content[6],
                BrazilianTime = TimeZoneInfo.ConvertTime(DateTime.Parse(content[3].Substring(1, 11)
                                + ' ' + content[3].Substring(13)), 
                                TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))
            };
        }

    }
}
