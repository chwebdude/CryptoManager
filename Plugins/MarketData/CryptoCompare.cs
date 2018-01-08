using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;
using Microsoft.Extensions.Caching.Memory;
using Model.Meta;
using NLog;

namespace Plugins.MarketData
{
    public class CryptoCompare : IMarketData
    {
        #region Static Fields and Constants

        private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static Dictionary<string, CoinMeta> CoinCache = new Dictionary<string, CoinMeta>();

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

        public async Task<CoinMeta> GetCoinInfo(string symbol)
        {
            if (CoinCache.Count == 0)
            {
                var client = new CryptoCompareClient();
                var data = await client.Coins.ListAsync();
                foreach (var coin in data.Coins)
                {
                    CoinCache.Add(coin.Value.Symbol, new CoinMeta()
                    {
                        Symbol = coin.Value.Symbol,
                        ImageUrl = data.BaseImageUrl + coin.Value.ImageUrl,
                        Name = coin.Value.CoinName,
                        Url = data.BaseImageUrl + coin.Value.Url
                    });
                }
            }

            if (CoinCache.ContainsKey(symbol))
                return CoinCache[symbol];
            Logger.Warn("Symbol {0} not present in cache", symbol);
            return null;
        }

        #endregion
    }
}
