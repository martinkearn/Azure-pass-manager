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
    public class ClaimController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public ClaimController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        // GET: api/Claim
        /// <summary>
        /// Checks request is valid and gets an unused code from storage and marks it as used before returning
        /// </summary>
        /// <param name="eventName">String containing a eventName that the code should belong to</param>
        /// <returns>Code</returns>
        [HttpGet]
        public async Task<IActionResult> Get(string eventName)
        {
            //ensure params are not null
            var eventNameNotNull = eventName ?? string.Empty;

            //Get first avaliable code which matches all criteria
            var codes = await _storeRepository.GetCodes();
            var code = codes
                .Where(x => x.EventName.ToLower() == eventName.ToLower())
                .Where(c => c.Claimed == false)
                .Where(c => c.Expiry >= DateTime.UtcNow)
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
                //no valid codes were found
                return NoContent();
            }

        }
    }
}