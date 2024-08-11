using System;

namespace ConfigurationLib.Dashboard.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
    }
}
