namespace APM.Bot
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public static class APMHelper
    {
        public static async Task<T> GetAzurePassCode<T>(Uri apmEndPoint)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, apmEndPoint);

                Debug.WriteLine($"Hitting: {apmEndPoint}");
                using (HttpResponseMessage response = await client.SendAsync(httpRequest))
                {
                    response.EnsureSuccessStatusCode();
                    string resp = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(resp);
                }
            }
        }
    }
}