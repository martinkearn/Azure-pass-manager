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
    public class EventsController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public EventsController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        /// <summary>
        /// Returns a list of event names for the specified owner
        /// </summary>
        /// <param name="owner">The owner to return events for</param>
        /// <returns>200 containing array of strings of event names</returns>
        [HttpGet]
        public async Task<IActionResult> Get(string owner)
        {
            var codes = await _storeRepository.GetCodes();

            //get list of unique eventNames for the owner
            var eventNames = codes
                .Where(x => x.Owner.ToLower() == owner.ToLower())
                .Select(x => x.EventName)
                .Distinct()
                .ToList();

            //for each unique eventName, create full event object and add to list to be returned
            var events = new List<Event>();
            foreach (var eventName in eventNames)
            {
                //all of this assumes all codes in the same EventName have common values
                var codesInEvent = codes
                    .Where(x => x.Owner.ToLower() == owner.ToLower())
                    .Where(x => x.EventName.ToLower() == eventName.ToLower())
                    .ToList();
                var expiry = codesInEvent.FirstOrDefault().Expiry;
                var url = Helpers.Helpers.EventNameToEventUrl(eventName);

                //create Event object
                var evnt = new Event()
                {
                    Codes = codesInEvent,
                    EventName = eventName,
                    Expiry = expiry,
                    Owner = owner,
                    Url = url
                };

                events.Add(evnt);
            }

            return Ok(events);
        }
        
    }
}