using System.Net;
using System.Text.RegularExpressions;

namespace client
{
    internal class AccessLogProcessor : ILogProcessor<AccessLogRegister>
    {
        //host ident authuser date request status bytes
        //45.138.145.131 - - [22/Dec/2020:21:33:59 +0100] "GET /index.php?option=com_contact&view=contact&id=1 HTTP/1.1" 200 9873 "-" "Mozilla/5.0(Linux;Android10;ASUS_Z01RD)AppleWebKit/537.36(KHTML,likeGecko)Chrome/85.0.4183.81MobileSafari/537.36" "-"

        private readonly int HOST_INDEX = 0;
        private readonly int IDENT_INDEX = 1;
        private readonly int AUTHUSER_INDEX = 2;
        private readonly int DATE_INDEX = 3;
        private readonly int REQUEST_INDEX = 4;
        private readonly int HTTPSTATUS_INDEX = 8;
        private readonly int BYTES_INDEX = 9;
        

        public void processLogLine(string line)
        {
            //string sourceIpPattern = @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)(\.(?!$)|$)){4}$";
            //Regex sourceIpRegex = new Regex(sourceIpPattern);
            //Console.WriteLine(sourceIpRegex.Match(line));
            Console.WriteLine(line);
        }

        public AccessLogRegister processLogLineAndReturnIt(string line)
        {
            var content = line.Split(' ');

            return new AccessLogRegister()
            {
                SourceIPAddress = content[HOST_INDEX] == "-" ? null : content[HOST_INDEX],

                StatusCode = content[HTTPSTATUS_INDEX] == "-" ? null : int.Parse(content[HTTPSTATUS_INDEX]),

                RequestBytes = content[BYTES_INDEX] == "-" ? null : int.Parse(content[BYTES_INDEX]),

                Url = content[6] == "-" ? null : content[6],

                BrazilianTime = content[DATE_INDEX] == "-" ? null : TimeZoneInfo.ConvertTime(DateTime.Parse(content[DATE_INDEX].Substring(1, 11)
                                                                    + ' ' + content[DATE_INDEX].Substring(13)), 
                                                                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))

            };


        }

    }
}
