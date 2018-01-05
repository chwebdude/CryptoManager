using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Plugins.MarketData
{
    public class CryptoCompare : IMarketData
    {
        #region Static Fields and Constants

        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods

        public async Task<decimal> GetCurrentRate(string baseCurrency, string currency)
        {
            return await Cache.GetOrCreate(baseCurrency + currency, async entry =>
            {
                var client = new CryptoCompareClient();
                Exception ex = null;

                // Try 3 times, or return 0
                for (int i = 0; i < 3; i++)
                {
                    try
                    {


                        var x = await client.Prices.SingleAsync(currency, new[] { baseCurrency }, true);
                        var res = x.First().Value;
                        return res;
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e);
                        ex = e;
                        await Task.Delay(5000);
                    }
                }
                Logger.Error(ex, "Failted to get Historic Market data");
                return 0;
            });
        }


        public async Task<decimal> GetHistoricRate(string baseCurrency, string currency, DateTime time)
        {
            var client = new CryptoCompareClient();
            Exception ex = null;

            // Try 3 times, or return a error
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var x = await client.Prices.HistoricalAsync(currency, new[] { baseCurrency }, time);

                    return x.First().Value.First().Value;

                }
                catch (Exception e)
                {
                    Logger.Warn(e);
                    ex = e;
                    await Task.Delay(5000);

                }
            }
            throw new Exception("Failted to get Historic Market data", ex);
        }

        #endregion
    }
}
