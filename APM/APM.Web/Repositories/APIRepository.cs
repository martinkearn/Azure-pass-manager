using APM.Domain;
using APM.Web.Interfaces;
using APM.Web.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APM.Web.Repositories
{
    public class ApiRepository : IApiRepository
    {
        private readonly AppSettings _appSettings;

        public ApiRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task StoreCodeBatch(CodeBatch codeBatch)
        {
            //setup HttpClient with content
            var httpClient = GetHttpClient();

            //setup body
            var content = new StreamContent(new MemoryStream(codeBatch.File));
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

            //construct full API endpoint uri
            DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
            var parameters = new Dictionary<string, string>
            {
                { "Expiry", codeBatch.Expiry.ToString(dtfi)},
                { "EventName", codeBatch.EventName },
                { "Owner", codeBatch.Owner }
            };
            var apiBaseUrl = $"{_appSettings.APIBaseUrl}/codes";
            var apiUri = QueryHelpers.AddQueryString(apiBaseUrl, parameters);

            //make request
            var responseMessage = await httpClient.PostAsync(apiUri, content);
        }

        public async Task<IEnumerable<Event>> GetEventsByOwner(string owner)
        {
            //setup HttpClient with content
            var httpClient = GetHttpClient();

            //construct full API endpoint uri
            var apiBaseUrl = $"{_appSettings.APIBaseUrl}/events/{owner}";

            //make request
            var responseMessage = await httpClient.GetAsync(apiBaseUrl);

            //cast to array of items
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            var responseArray = JArray.Parse(responseString);
            var items = new List<Event>();
            foreach (var response in responseArray)
            {
                var item = JsonConvert.DeserializeObject<Event>(response.ToString());
                items.Add(item);
            }

            return items;
        }

        public async Task<Event> GetEventByEventName(string eventName)
        {
            //setup HttpClient with content
            var httpClient = GetHttpClient();

            //construct full API endpoint uri
            var apiBaseUrl = $"{_appSettings.APIBaseUrl}/event/{eventName}";

            //make request
            var responseMessage = await httpClient.GetAsync(apiBaseUrl);

            //cast to item
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            var evnt = JsonConvert.DeserializeObject<Event>(responseString.ToString());

            return evnt;
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            var apiUrlBase = _appSettings.APIBaseUrl;
            httpClient.BaseAddress = new Uri(apiUrlBase);
            return httpClient;
        }
    }
}
