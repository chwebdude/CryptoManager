using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Caching.Memory;

namespace Plugins.MarketData
{
    public class CryptoCompare : IMarketData
    {
        #region Static Fields and Constants

        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        #endregion

        #region Public Methods

        public async Task<decimal> GetCurrentRate(string baseCurrency, string currency)
        {
            return await Cache.GetOrCreate(baseCurrency + currency, async entry =>
            {
                var client = new CryptoCompareClient();
                var x = await client.Prices.SingleAsync(currency, new[] {baseCurrency}, true);
                var res = x.First().Value;
                return res;
            });
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
