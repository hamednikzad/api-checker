using System.Collections.Generic;

namespace ApiChecker
{
    public class AppConfig
    {
        public int IntervalInSeconds { get; set; }
        public List<ApiAddress> ApiAddresses { get; set; }
    }
}