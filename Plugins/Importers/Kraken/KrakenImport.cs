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
            var deposits = api.GetLedgers(null, null, "deposit");
            if(assetsCache == null)
                BuildPairCache(api);
            

            var transactions = new List<CryptoTransaction>();

            foreach (var deposit in deposits.Ledger)
            {
                var d = deposit.Value;
                var key = d.Refid;
                var dateTime = Helpers.UnixTimeStampToDateTime(d.Time);
                var amount = d.Amount;
                var currency = assetsCache[d.Asset];
                transactions.Add(CryptoTransaction.NewIn(key, dateTime, exchange.Id, "In", amount, currency, string.Empty, String.Empty, String.Empty));
            }

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
