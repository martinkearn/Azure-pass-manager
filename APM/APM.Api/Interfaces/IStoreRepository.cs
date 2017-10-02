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

        Task StoreCodes(List<Code> codes);

        Task DeleteCode(string codeId);

        Task DeleteCodes(string codeIds);

        Task<Code> GetCode(string codeId);

        Task<List<Code>> GetCodes();
    }
}
