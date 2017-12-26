using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;

namespace Plugins.Importers.Binance
{
    class BinanceImport : IImporter
    {
        public Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            throw new NotImplementedException();
        }

        public ExchangeMeta GetExchangeMeta()
        {
            return new ExchangeMeta()
            {
                ExchangeId = Model.Enums.Exchange.Binance,
                Name = "Binance",
                LabelPublicKey = "API Key",
                LabelPrivateKey = "Secret"

            };
        }

        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.Binance;
    }
}
