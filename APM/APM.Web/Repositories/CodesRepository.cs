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
    public class CodesRepository : ICodesRepository
    {
        private readonly AppSettings _appSettings;

        public CodesRepository(IOptions<AppSettings> appSettings)
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
            var codeBatchAPIUrl = $"{_appSettings.APIBaseUrl}/codes";
            var apiUri = QueryHelpers.AddQueryString(codeBatchAPIUrl, parameters);

            //make request
            var responseMessage = await httpClient.PostAsync(apiUri, content);
        }
    }
}
