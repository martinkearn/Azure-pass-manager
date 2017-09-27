using System;
using System.Collections.Generic;
using System.Text;

namespace APM.Domain
{
    public class Event
    {
        public string EventName { get; set; }

        public string Owner { get; set; }

        public DateTime Expiry { get; set; }

        public string Url { get; set; }

        public List<Code> Codes { get; set; }
    }
}
