using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;

namespace Plugins.Importers.Kraken
{
    public class KrakenImport : IImporter
    {
        private IMarketData _marketData;

        public KrakenImport(IMarketData marketData)
        {
            _marketData = marketData;
        }

        private static Dictionary<string, string> assetsCache;

        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
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
                var d = deposit.Value;
                var key = d.Refid;
                var dateTime = Helpers.UnixTimeStampToDateTime(d.Time);
                var amount = d.Amount;
                var currency = assetsCache[d.Asset];
                transactions.Add(CryptoTransaction.NewIn(key, dateTime, exchange.Id, "In", amount, currency, string.Empty, String.Empty, String.Empty));
            }

            await Task.Delay(2000);

            var withdrawals = api.GetLedgers(null, null, "withdrawal");
            foreach (var withdrwaw in withdrawals.Ledger)
            {
                var w = withdrwaw.Value;
                var key = w.Refid;
                var dateTime = Helpers.UnixTimeStampToDateTime(w.Time);
                var amount = -1 * w.Amount;
                var currency = assetsCache[w.Asset];
                var fee = w.Fee;
                transactions.Add(CryptoTransaction.NewOut(key, dateTime, exchange.Id, "Out", amount, currency, fee, currency, string.Empty, String.Empty, String.Empty));
            }

            await Task.Delay(2000);

            var trades = api.GetTradesHistory(null, true);

            return transactions;
        }

        private static void BuildPairCache(Kraken api)
        {
            var assets = api.GetAssetInfo();
            assetsCache = new Dictionary<string, string>();
            foreach (var assetInfo in assets)
            {
                assetsCache.Add(assetInfo.Key, assetInfo.Value.Altname);
            }

            // Overrides
            assetsCache["XXBT"] = "BTC";
        }

        public ExchangeMeta GetExchangeMeta() => new ExchangeMeta()
        {
            ExchangeId = Model.Enums.Exchange.Kraken,
            Name = "Kraken",
            LabelPublicKey = "API Key",
            LabelPrivateKey = "API Private Key"
        };


        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.Kraken;
    }
}
