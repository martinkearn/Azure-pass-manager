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
    public class EventController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public EventController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        /// <summary>
        /// Returns a single event (with codes)
        /// </summary>
        /// <param name="eventName">The name of the event to return</param>
        /// <param name="Owner">String representing the alias of the user who owns the code</param>
        /// <returns>200 containing an Event</returns>
        [HttpGet]
        public async Task<IActionResult> Get(string owner, string eventName)
        {
            //get all codes
            var allCodes = await _storeRepository.GetCodes(owner);

            //filter codes on EventName
            var codesInEvent = allCodes
                .Where(x => x.EventName.ToLower() == eventName.ToLower())
                .ToList();

            //all of this assumes all codes in the same EventName have common values
            var expiry = codesInEvent.FirstOrDefault().Expiry;
            var url = Helpers.Helpers.EventNameToEventUrl(eventName);
            var codes = codesInEvent;

            //create Event object
            var evnt = new Event()
            {
                Codes = codes,
                EventName = eventName,
                Expiry = expiry,
                Owner = owner,
                Url = url
            };

            return Ok(evnt);
        }

        /// <summary>
        /// Deletes an event and its codes
        /// </summary>
        /// <param name="eventName">The name of the event to delete</param>
        /// <param name="Owner">String representing the alias of the user who owns the code</param>
        /// <returns>202 if successful</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(string owner, string eventName)
        {
            //get all codes
            var allCodes = await _storeRepository.GetCodes(owner);

            //filter codes on EventName
            var codesInEvent = allCodes
                .Where(x => x.EventName.ToLower() == eventName.ToLower())
                .Select(x => x.PromoCode)
                .ToList();

            //cast to list of code ids
            var codeIdsInEvent = string.Join(",", codesInEvent);

            //delete
            await _storeRepository.DeleteCodes(owner, codeIdsInEvent);

            //return 202 (resource marked for deletion)
            return StatusCode(202);
        }
    }
}