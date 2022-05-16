namespace client
{
    internal class AccessLogProcessor
    {
        private readonly int HOST_INDEX = 0;
        private readonly int DATE_INDEX = 3;
        private readonly int URL_INDEX = 6;
        private readonly int HTTPSTATUS_INDEX = 8;
        private readonly int BYTES_INDEX = 9;

        public void processLogLine(string line)
        {
            throw new NotImplementedException();
        }

        public AccessLogRegister? processLogLineAndReturnIt(string line)
        {
            var content = line.Split(' ');

            return new AccessLogRegister()
            {
                SourceIPAddress = (content[HOST_INDEX] == "-" || String.IsNullOrEmpty(content[HOST_INDEX])) ? null : content[HOST_INDEX],

                StatusCode = (content[HTTPSTATUS_INDEX] == "-" || String.IsNullOrEmpty(content[HTTPSTATUS_INDEX])) ? null : int.Parse(content[HTTPSTATUS_INDEX]),

                RequestBytes = (content[BYTES_INDEX] == "-" || String.IsNullOrEmpty(content[BYTES_INDEX])) ? null : int.Parse(content[BYTES_INDEX]),

                Url = (content[URL_INDEX] == "-" || String.IsNullOrEmpty(content[URL_INDEX])) ? null : content[URL_INDEX],

                BrazilianTime = (content[DATE_INDEX] == "-" || String.IsNullOrEmpty(content[DATE_INDEX])) ? null : TimeZoneInfo.ConvertTime(DateTime.Parse(content[DATE_INDEX].Substring(1, 11)
                                                                    + ' ' + content[DATE_INDEX].Substring(13)), 
                                                                    TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"))

            };
        }
    }
}
