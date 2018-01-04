using System;
using System.Threading.Tasks;

namespace Plugins
{
    public interface IMarketData
    {
        Task<decimal> GetCurrentRate(string baseCurrency, string currency);
        Task<decimal> GetHistoricRate(string baseCurrency, string currency, DateTime time);
    }
}
