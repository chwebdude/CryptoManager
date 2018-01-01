using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Info.Blockchain.API.BlockExplorer;
using Info.Blockchain.API.Client;
using Model.DbModels;
using Model.Meta;

namespace Plugins.Importers.Blockchain
{
    public class BlockchainImport:IImporter
    {
        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            var api = new BlockExplorer();
            var xpub = await api.GetXpub(exchange.PublicKey); 
            foreach (var xpubTransaction in xpub.Transactions)
            {
                //var allPrevious = xpubTransaction.Outputs.Sum(o => o..PreviousOutput.Value.Satoshis);
                var allOut = xpubTransaction.Outputs.Sum(o => o.Value.Satoshis);

                var previousOutput = xpubTransaction.Inputs.Sum(i => i.PreviousOutput.Value.Satoshis);

            }
            throw new NotImplementedException();
        }

        public ExchangeMeta GetExchangeMeta()
        {
            return new ExchangeMeta()
            {
                ExchangeId = Model.Enums.Exchange.Blockchain,
                Name = "Blockchain.info",
                LabelPublicKey = "xPub"
            };
        }

        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.Blockchain;
    }
}
