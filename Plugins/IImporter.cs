using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;

namespace Plugins
{
    public interface IImporter
    {                
        Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange);
    }
}
