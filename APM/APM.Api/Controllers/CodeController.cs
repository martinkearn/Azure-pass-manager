using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APM.Api.Interfaces;
using APM.Domain;

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
        /// Gets the matching code and returns it
        /// </summary>
        /// <param name="eventName">String containing a eventName that the code should belong to</param>
        /// <param name="promoCode">String containing the promoCode representing the code</param>
        /// <returns>Code</returns>
        [HttpGet]
        public async Task<IActionResult> Get(string eventName, string promoCode)
        {
            //ensure params are not null
            var eventNameNotNull = eventName ?? string.Empty;

            //Get first avaliable code which matches all criteria
            var codes = await _storeRepository.GetCodes();
            var code = codes
                .Where(x => x.EventName.ToLower() == eventName.ToLower())
                .Where(c => c.PromoCode.ToLower() == promoCode.ToLower())
                .FirstOrDefault();

            if (code != null)
            {
                return Ok(code);
            }
            else
            {
                //no valid codes were found
                return NoContent();
            }

        }

        /// <summary>
        /// Updates a code in storage by replacing existing one with what was passed in
        /// </summary>
        /// <param name="code">A code object which is to be updated</param>
        /// <returns>The code object which was updated</returns>
        [HttpPut]
        public async Task<IActionResult> Put(Code code)
        {
            if (code == null) return BadRequest("Invalid data passed in");

            //replace code in storage
            await _storeRepository.StoreCode(code);

            return Ok(code);
        }
    }
}