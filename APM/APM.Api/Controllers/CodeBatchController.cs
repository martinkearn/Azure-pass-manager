using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace APM.Api.Controllers
{
    [Produces("application/json")]
    [ApiVersion("0.1")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class CodeBatchController : Controller
    {
        public IConfiguration _configuration { get; set; }

        public CodeBatchController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: api/CodeBatch
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
    }
}
