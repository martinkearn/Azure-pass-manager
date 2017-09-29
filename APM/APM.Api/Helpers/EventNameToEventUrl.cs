using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APM.Api.Helpers
{
    public static partial class Helpers
    {

        public static string EventNameToEventUrl(string eventName)
        {
            var eventUrl = eventName;
            eventUrl = eventUrl.ToLower();
            eventUrl = eventUrl.Replace(" ", "");
            eventUrl = eventUrl.TrimEnd();
            eventUrl = eventUrl.TrimStart();
            return $"/{eventUrl}";
        }
    }
}
