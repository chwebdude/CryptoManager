using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;
using Newtonsoft.Json;
using NLog;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace Plugins.Importers.Blockchain
{
    public class BlockchainImport : IImporter
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();

        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            var list = new List<CryptoTransaction>();

            var client = new RestClient("https://blockchain.info/");
            var request = new RestRequest("multiaddr", Method.GET);
            request.AddParameter("active", exchange.PublicKey);
            Logger.Trace("GET blockchain.info for xPub");

            var res = await client.Execute(request);
            if (res.IsSuccess)
            {
                var response = JsonConvert.DeserializeObject<BlockchainResponse>(res.Content);
                foreach (var transaction in response.Txs)
                {
                    if (transaction.Result > 0)
                    {
                        // Is Input
                        list.Add(
                            CryptoTransaction.NewIn(
                               transaction.Hash,
                                Helpers.UnixTimeStampToDateTime(transaction.Time),
                                exchange.Id,
                                "Receive",
                                transaction.Result / 100000000,
                                "BTC",
                               transaction.Inputs.First().Prev_Out.Addr,
                               transaction.Out.First(o => o.XPub.M == exchange.PublicKey).Addr,
                                transaction.Hash
                            ));
                    }
                    else
                    {
                        // Is Output
                        var amount = (-1 * transaction.Result - transaction.Fee) / 100000000;
                        var fee = transaction.Fee / 100000000;
                        list.Add(
                                                CryptoTransaction.NewOut(
                                                    transaction.Hash,
                                                    Helpers.UnixTimeStampToDateTime(transaction.Time),
                                                    exchange.Id,
                                                    "Sent",
                                                    amount,
                                                    "BTC",
                                                    fee,
                                                    "BTC",
                                                    transaction.Inputs.First().Prev_Out.Addr,
                                                    transaction.Out.First(o => o.XPub == null || o.XPub.M != exchange.PublicKey).Addr,
                                                    transaction.Hash
                                                ));
                    }
                }
            }
            else
            {
                Logger.Error(res.StatusDescription);
                Logger.Error(res.Content);
                throw new Exception(res.StatusDescription);
            }

            return list;

        }

        public ExchangeMeta GetExchangeMeta()
        {
            return new ExchangeMeta
            {
                ExchangeId = Model.Enums.Exchange.Blockchain,
                Name = "Blockchain.info",
                LabelPublicKey = "xPub"
            };
        }

        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.Blockchain;
    }
}
