using System;

namespace WebApiContrib.Caching
{
    public class ThrottleEntry
    {
        public DateTime PeriodStart { get; set; }
        public long Requests { get; set; }

        public ThrottleEntry()
        {
            PeriodStart = DateTime.UtcNow;
            Requests = 0;
        }
    }
}