using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APM.Api.Models;
using Microsoft.Extensions.Options;

namespace APM.Api.Controllers
{
    [Produces("application/json")]
    [ApiVersion("0.1")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class ValuesController : Controller
    {
        //public IConfiguration _configuration { get; set; }
        private readonly AppSettings _appSettings;

        private readonly AppSecretSettings _appSecretSettings;
        
        public ValuesController(IOptions<AppSettings> appSettings, IOptions<AppSecretSettings> appSecretSettings)
        {
            _appSettings = appSettings.Value;
            _appSecretSettings = appSecretSettings.Value;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var container = _appSettings.TableStorageContainerName;
            var key = _appSettings.TableStoragePartitionKey;
            var cs = _appSecretSettings.TableStorageConnectionString;
            return new string[] { container, key, cs };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
