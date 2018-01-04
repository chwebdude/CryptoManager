using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;

namespace Plugins.MarketData
{
    public class CryptoCompare : IMarketData
    {
        #region Public Methods

        public async Task<decimal> GetCurrentRate(string baseCurrency, string currency)
        {
            var client = new CryptoCompareClient();
            var x = await client.Prices.SingleAsync(currency, new[] {baseCurrency}, true);
            return x.First().Value;
        }

        public async Task<decimal> GetHistoricRate(string baseCurrency, string currency, DateTime time)
        {
            var client = new CryptoCompareClient();
            var x = await client.Prices.HistoricalAsync(currency, new[] {baseCurrency}, time);
            return x.First().Value.First().Value;
        }

        #endregion
    }
}
