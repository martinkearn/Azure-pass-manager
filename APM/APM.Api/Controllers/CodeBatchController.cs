using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using APM.Api.Interfaces;
using APM.Domain;

namespace APM.Api.Controllers
{
    [Produces("application/json")]
    [ApiVersion("0.1")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    public class CodeBatchController : Controller
    {
        private readonly IStoreRepository _storeRepository;

        public CodeBatchController(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        // POST: api/CodeBatch
        /// <summary>
        /// Loads the contents of a CSV file to storage as Codes
        /// </summary>
        /// <param name="Expiry">DateTime representing the expiration of the codes in the batch. Format YYYY-MM-DDTHH:MM:SS i.e. 2017-09-25T00:00:00</param>
        /// <param name="EventName">String representing teh name of the event</param>
        /// <param name="Password">Password required to claim a code</param>
        /// <param name="ValidFrom">DateTime representing the date/time when code can be claim from. Format YYYY-MM-DDTHH:MM:SS i.e. 2017-09-25T00:00:00</param>
        /// <param name="ValidUntil">DateTime representing the date/time when code can be claim until. Format YYYY-MM-DDTHH:MM:SS i.e. 2017-09-25T00:00:00</param>
        /// <param name="Owner">String representing the alias of the user who owns the code</param>
        /// <body>A file (CSV) containg comma seperated list of codes</body>
        /// <returns>Array of Codes</returns>
        [HttpPost]
        public async Task<IActionResult> Post(CodeBatch codeBatch)
        {
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
                        var code = new Code()
                        {
                            PromoCode = promoCode ?? "No Code",
                            Claimed = false,
                            EventName = codeBatch.EventName ?? "No Event Name",
                            Expiry = codeBatch.Expiry,
                            Owner = codeBatch.Owner ?? string.Empty,
                            Password = codeBatch.Password ?? string.Empty,
                            ValidFrom = codeBatch.ValidFrom,
                            ValidUntil = codeBatch.ValidUntil,
                            TimeZoneOffsetFromUTC = codeBatch.TimeZoneOffsetFromUTC
                        };

                        codesList.Add(code);
                    }
                }
            }

            // Store codes
            foreach (var code in codesList)
            {
                await _storeRepository.StoreCode(code);
            }

            return Ok(codesList);
        }
    }
}
