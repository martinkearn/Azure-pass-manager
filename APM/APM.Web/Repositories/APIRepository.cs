using APM.Domain;
using APM.Web.Interfaces;
using APM.Web.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
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
    public class APIRepository : IAPIRepository
    {
        private readonly AppSettings _appSettings;

        public APIRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task StoreCodeBatch(CodeBatch codeBatch)
        {
            //setup HttpClient with content
            var httpClient = new HttpClient();
            var apiUrlBase = _appSettings.APIBaseUrl;
            httpClient.BaseAddress = new Uri(apiUrlBase);

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
            var httpClient = new HttpClient();
            var apiUrlBase = _appSettings.APIBaseUrl;
            httpClient.BaseAddress = new Uri(apiUrlBase);

            //construct full API endpoint uri
            var apiBaseUrl = $"{_appSettings.APIBaseUrl}/events/{owner}";

            //make request
            var responseMessage = await httpClient.GetAsync(apiBaseUrl);

            return null;
        }
    }
}
