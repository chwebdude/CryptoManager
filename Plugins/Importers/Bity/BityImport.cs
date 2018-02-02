using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;
using Plugins.MarketData;

namespace Plugins.Importers.Bity
{
    public class BityImport : IImporter
    {
        private readonly IMarketData _marketData;

        public BityImport(IMarketData marketData)
        {
            _marketData = marketData;
        }

        public Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            throw new NotImplementedException();
        }

        public ExchangeMeta GetExchangeMeta() => new ExchangeMeta()
        {
            ExchangeId = Model.Enums.Exchange.Bity,
            Name = "Bity.com",
            LabelPublicKey = "Username",
            LabelPassphrase = "Password"
        };

        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.Bity;
    }
}
