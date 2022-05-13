using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class AccessLogRegister
    {
        public AccessLogRegister()
        {

        }

        public AccessLogRegister(string sourceIPAddress, string destinationIPAddress, DateTime brazilianTime, string url, int statusCode, int requestBytes)
        {
            SourceIPAddress = sourceIPAddress;
            DestinationIPAddress = destinationIPAddress;
            BrazilianTime = brazilianTime;
            Url = url;
            StatusCode = statusCode;
            RequestBytes = requestBytes;
        }

        public int Id { get; set; }
        public string? SourceIPAddress { get; set; }
        public string? DestinationIPAddress { get; set; }
        public DateTime? BrazilianTime { get; set; }
        public string? Url { get; set; }
        public int? StatusCode { get; set; }
        public int? RequestBytes { get; set; }
    }
}
