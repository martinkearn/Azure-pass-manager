using APM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APM.Web.Interfaces
{
    public interface IApiRepository
    {
        Task StoreCodeBatch(CodeBatch codeBatch);

        Task<IEnumerable<Event>> GetEventsByOwner(string owner);

        Task<Event> GetEventByEventName(string owner, string eventName);

        Task DeleteEventByEventName(string owner, string eventName);
    }
}
