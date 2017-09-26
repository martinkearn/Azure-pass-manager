using System;
using System.Collections.Generic;
using System.Text;

namespace APM.Domain
{
    public class CodeBatch
    {
        public DateTime Expiry { get; set; }

        public string EventName { get; set; }

        public string Password { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidUntil { get; set; }

        public string Owner { get; set; }

        public byte[] File { get; set; }
        public int TimeZoneOffsetFromUTC { get; set; }
    }
}
