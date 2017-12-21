using System;

namespace APM.Domain
{
    public class Code
    {
        public string PromoCode { get; set; }

        public DateTime Expiry { get; set; }

        public bool Claimed { get; set; }

        public string EventName { get; set; }

        public string Owner { get; set; }

        public DateTime AvaliableFrom { get; set; }

        public DateTime AvaliableUntil { get; set; }
    }
}
