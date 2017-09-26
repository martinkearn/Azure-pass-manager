using System;

namespace APM.Domain
{
    public class Code
    {
        public string PromoCode { get; set; }

        public DateTime Expiry { get; set; }

        public bool Claimed { get; set; }

        public string EventName { get; set; }

        public string Password { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidUntil { get; set; }

        public string Owner { get; set; }
    }
}
