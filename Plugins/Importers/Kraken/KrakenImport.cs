using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;
using Exchange = Model.Enums.Exchange;

namespace Plugins.Importers.Kraken
{
    /// <summary>
    /// Kraken Importer.
    /// </summary>
    /// <seealso cref="Plugins.IImporter" />
    public class KrakenImport : IImporter
    {
        #region Fields

        /// <summary>
        /// The assets cache.
        /// </summary>
        private static Dictionary<string, string> assetsCache = new Dictionary<string, string>();

        /// <summary>
        /// The market data.
        /// </summary>
        private readonly IMarketData _marketData;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KrakenImport" /> class.
        /// </summary>
        /// <param name="marketData">The market data.</param>
        public KrakenImport(IMarketData marketData)
        {
            _marketData = marketData;
        }

        #endregion

        #region Properties

        public Exchange Exchange => Exchange.Kraken;

        #endregion

        #region Interface Implementations

        /// <summary>
        /// Gets the transactions.
        /// </summary>
        /// <param name="exchange">The exchange.</param>
        /// <returns>The transactions.</returns>
        /// <exception cref="NotImplementedException">On incorrect trade type.</exception>
        public async Task<IEnumerable<CryptoTransaction>> GetTransactionsAsync(Model.DbModels.Exchange exchange)
        {
            var api = new Kraken(exchange.PublicKey, exchange.PrivateKey);
            if (assetsCache == null)
            {
                BuildPairCache(api);
                await Task.Delay(2000);
            }

            var transactions = new List<CryptoTransaction>();

            var deposits = api.GetLedgers(null, null, "deposit");
            foreach (var deposit in deposits.Ledger)
            {
                var d        = deposit.Value;
                var key      = d.Refid;
                var dateTime = Helpers.UnixTimeStampToDateTime(d.Time);
                var amount   = d.Amount;
                var currency = assetsCache[d.Asset];
                transactions.Add(CryptoTransaction.NewIn(key, dateTime, exchange.Id, "In", amount, currency, string.Empty, string.Empty, string.Empty));
            }

            await Task.Delay(2000);

            var withdrawals = api.GetLedgers(null, null, "withdrawal");
            foreach (var withdrwaw in withdrawals.Ledger)
            {
                var w        = withdrwaw.Value;
                var key      = w.Refid;
                var dateTime = Helpers.UnixTimeStampToDateTime(w.Time);
                var amount   = -1 * w.Amount;
                var currency = assetsCache[w.Asset];
                var fee      = w.Fee;
                transactions.Add(CryptoTransaction.NewOut(key, dateTime, exchange.Id, "Out", amount, currency, fee, currency, string.Empty, string.Empty, string.Empty));
            }

            await Task.Delay(2000);

            var trades = api.GetTradesHistory(null, true);
            foreach (var trade in trades.Trades)
            {
                var currency1 = assetsCache[trade.Value.Pair.Split('Z').First()];
                var currency2 = assetsCache[trade.Value.Pair.Split('Z').Last()];
                var fee       = trade.Value.Fee;
                var cost      = trade.Value.Cost;
                var volume    = trade.Value.Vol;
                var dateTime  = Helpers.UnixTimeStampToDateTime(trade.Value.Time);
                var fiatRate  = await _marketData.GetHistoricRate("CHF", currency1, dateTime);

                switch (trade.Value.Type)
                {
                    case "buy":
                        transactions.Add(
                                         CryptoTransaction.NewTrade(
                                                                    trade.Key, dateTime, exchange.Id, "Kraken Buy", volume, currency1, fee, currency2, cost,
                                                                    currency2, fiatRate));

                        break;
                    case "sell":
                        transactions.Add(
                                         CryptoTransaction.NewTrade(
                                                                    trade.Key, dateTime, exchange.Id, "Kraken Sell", cost, currency2, fee, currency2, volume,
                                                                    currency1, fiatRate));

                        break;
                    default: throw new NotImplementedException();
                }
            }

            return transactions;
        }

        /// <summary>
        /// Gets the exchange meta.
        /// </summary>
        /// <returns>ExchangeMeta.</returns>
        public ExchangeMeta GetExchangeMeta()
        {
            return new ExchangeMeta { ExchangeId = Exchange.Kraken, Name = "Kraken", LabelPublicKey = "API Key", LabelPrivateKey = "API Private Key" };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the pair cache.
        /// </summary>
        /// <param name="api">The API.</param>
        private static void BuildPairCache(Kraken api)
        {
            var assets = api.GetAssetInfo();
            assetsCache = new Dictionary<string, string>();
            foreach (var assetInfo in assets) assetsCache.Add(assetInfo.Key, assetInfo.Value.Altname);

            // Overrides
            assetsCache["XXBT"] = "BTC";

            // Fiat
            assetsCache.Add("USD", "USD");
            assetsCache.Add("EUR", "EUR");
        }

        #endregion
    }
}