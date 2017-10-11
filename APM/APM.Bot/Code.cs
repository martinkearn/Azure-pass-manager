using System;

namespace APM.Bot
{
    // We can't add reference to APM.Domain as it's .NET Core and BF is .NET 4.6 :(
    public class Code
    {
        public string PromoCode { get; set; }

        public DateTime Expiry { get; set; }

        public bool Claimed { get; set; }

        public string EventName { get; set; }

        public string Owner { get; set; }
    }
}