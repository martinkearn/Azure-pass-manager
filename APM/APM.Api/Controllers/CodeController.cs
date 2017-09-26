using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Api.Interfaces;

namespace APM.Api.Controllers
{
    [Produces("application/json")]
    [ApiVersion("0.1")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class CodeController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public CodeController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        // GET: api/Code
        /// <summary>
        /// Checks request is valid and gets an unused code from storage and marks it as used before returning
        /// </summary>
        /// <param name="eventName">String containing a eventName that the code should belong to</param>
        /// <param name="password">String containing the password for the code</param>
        /// <param name="timeZoneOffsetFromUTC">An Int representing the offset compared to UTC for the current datetime. For example in the UK during summer time, this would be set to 1, in Seattle -8</param>
        /// <returns>Code</returns>
        [HttpGet]
        public async Task<IActionResult> Get(string eventName, string password, int timeZoneOffsetFromUTC)
        {
            //ensure params are not null
            var passwordNotNull = password ?? string.Empty;
            var eventNameNotNull = eventName ?? string.Empty;
            var dateTimeNow = DateTime.UtcNow.AddHours(timeZoneOffsetFromUTC);

            //Get first avaliable code which matches all criteria
            var codes = await _storeRepository.GetCodes();
            var code = codes
                .Where(c => c.Claimed == false)
                .Where(c => c.EventName.ToLower() == eventNameNotNull.ToLower())
                .Where(c => c.Password.ToLower() == passwordNotNull.ToLower())
                .Where(c => c.ValidFrom <= dateTimeNow)
                .Where(c => c.ValidUntil >= dateTimeNow)
                .FirstOrDefault();

            if (code != null)
            {
                //update code as used
                code.Claimed = true;

                //replace code in storage
                await _storeRepository.StoreCode(code);

                return Ok(code);
            }
            else
            {
                //no codes matching eventName, password and time window were found
                return NoContent();
            }

        }


    }
}