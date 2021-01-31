using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;

namespace Plugins
{
    public interface IImporter
    {                
        Task<IEnumerable<CryptoTransaction>> GetTransactionsAsync(Exchange exchange);
        ExchangeMeta GetExchangeMeta();
        Model.Enums.Exchange Exchange { get; }
    }
}
