using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APM.Api.Interfaces;
using APM.Domain;
using System.Globalization;

namespace APM.Api.Controllers
{
    [Produces("application/json")]
    [ApiVersion("0.1")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class CodesController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public CodesController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        // POST: api/Codes
        /// <summary>
        /// Loads the contents of a CSV file to storage as Codes
        /// </summary>
        /// <param name="Expiry">DateTime representing the expiration of the codes in the batch. Format YYYY-MM-DDTHH:MM:SS i.e. 2017-09-25T00:00:00</param>
        /// <param name="EventName">String representing the name of the event</param>
        /// <param name="Owner">String representing the alias of the user who owns the code</param>
        /// <body>A file (CSV) containg comma seperated list of codes</body>
        /// <returns>200 containing an array of Codes</returns>
        [HttpPost]
        public async Task<IActionResult> Post(CodeBatch codeBatch)
        {
            //check that the eventname is unique
            //get all codes
            var allCodes = await _storeRepository.GetCodes();

            //filter codes on EventName
            var codesInEvent = allCodes
                .Where(x => x.EventName.ToLower() == codeBatch.EventName.ToLower())
                .ToList();

            if (codesInEvent.Count > 0)
            {
                return BadRequest("Event Name is not unique");
            }

            // Get file into an array of strings
            byte[] file = Helpers.Helpers.ReadFileStream(Request.Body);
            codeBatch.File = file;
            var fileString = System.Text.Encoding.Default.GetString(codeBatch.File);
            string[] stringSeparators = new string[] { "\r\n" };
            string[] lines = fileString.Split(stringSeparators, StringSplitOptions.None);

            // add each code to list
            var codesList = new List<Code>();
            foreach (var line in lines)
            {
                // split line to individual codes
                string[] promoCodes = line.Split(',');
                foreach (var promoCode in promoCodes)
                {
                    if (!string.IsNullOrEmpty(promoCode))
                    {

                        DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
                        var code = new Code()
                        {
                            PromoCode = promoCode ?? string.Empty,
                            Claimed = false,
                            EventName = codeBatch.EventName ?? string.Empty,
                            Expiry = Convert.ToDateTime(codeBatch.Expiry, dtfi),
                            Owner = codeBatch.Owner ?? string.Empty
                        };

                        codesList.Add(code);
                    }
                }
            }

            // Store codes
            await _storeRepository.StoreCodes(codesList);

            return Ok(codesList);
        }


    }
}
