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

        Task DeleteCode(string owner, string codeId);

        Task DeleteCodes(string owner, string codeIds);

        Task<Code> GetCode(string owner, string codeId);

        Task<List<Code>> GetCodes(string owner);
    }
}
