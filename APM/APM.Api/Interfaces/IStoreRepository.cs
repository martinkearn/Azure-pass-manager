using APM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APM.Api.Interfaces
{
    public interface IStoreRepository
    {
        Task StoreCode(Code code);

        Task DeleteCode(string codeId);

        Task<Code> GetCode(string codeId);

        Task<List<Code>> GetCodes();
    }
}
