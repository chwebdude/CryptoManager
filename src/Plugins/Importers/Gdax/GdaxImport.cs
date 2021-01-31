using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GDAXClient.Authentication;
using GDAXClient.Services.Fundings;
using Model.DbModels;
using Model.Meta;

namespace Plugins.Importers.Gdax
{
    public class GdaxImport : IImporter
    {
        private readonly IMarketData _marketData;

        public GdaxImport(IMarketData marketData)
        {
            _marketData = marketData;
        }

        public async Task<IEnumerable<CryptoTransaction>> GetTransactionsAsync(Exchange exchange)
        {
            var authenticator = new Authenticator(exchange.PublicKey, exchange.PrivateKey, exchange.Passphrase);
            var client = new GDAXClient.GDAXClient(authenticator);
            var transactions = new List<CryptoTransaction>();
            var fills = await client.FillsService.GetAllFillsAsync();
            foreach (var fill in fills)
            {
                foreach (var fillResponse in fill)
                {
                    var buyCurrency = fillResponse.Side == "buy"
                        ? fillResponse.Product_id.Split('-')[0]
                        : fillResponse.Product_id.Split('-')[1];
                    var sellCurrency = fillResponse.Side == "buy"
                        ? fillResponse.Product_id.Split('-')[1]
                        : fillResponse.Product_id.Split('-')[0];

                    var buyAmount = fillResponse.Side == "buy"
                        ? fillResponse.Size
                        : fillResponse.Size * fillResponse.Price;

                    var sellAmount = fillResponse.Side == "buy"
                        ? fillResponse.Size * fillResponse.Price
                        : fillResponse.Size;

                    transactions.Add(CryptoTransaction.NewTrade(fillResponse.Trade_id.ToString(), fillResponse.Created_at,
                        exchange.Id,
                        "",
                        buyAmount,
                        buyCurrency,
                        fillResponse.Fee,
                        "EUR",
                        sellAmount,
                        sellCurrency,
                        await _marketData.GetHistoricRate("CHF", buyCurrency, fillResponse.Created_at)

                        ));
                }
            }

            await Task.Delay(200);
            var accounts = await client.AccountsService.GetAllAccountsAsync();
            await Task.Delay(200);
            foreach (var account in accounts)
            {
                var histories = await client.AccountsService.GetAccountHistoryAsync(account.Id.ToString());
                await Task.Delay(200);
                foreach (var history in histories)
                {
                    var transfers = history.Where(h => h.Type == "transfer");

                    foreach (var transfer in transfers)
                    {
                        if (transfer.Amount > 0)
                        {
                            transactions.Add(CryptoTransaction.NewIn(
                                transfer.Id,
                                transfer.Created_at,
                                exchange.Id,
                                "Transfer from Coinbase",
                                transfer.Amount,
                                account.Currency,
                                "Coinbase",
                                "GDAX",
                                transfer.Id
                                ));
                        }
                        else
                        {
                            transactions.Add(CryptoTransaction.NewOut(
                                transfer.Id,
                                transfer.Created_at,
                                exchange.Id,
                                "To Coinbase",
                                transfer.Amount * -1,
                                account.Currency,
                                0,
                                account.Currency,
                                "GDAX",
                                "Coinbase",
                                transfer.Id
                                ));
                        }
                    }
                }
            }

            return transactions;
        }

        public ExchangeMeta GetExchangeMeta() => new ExchangeMeta()
        {
            Name = "GDAX",
            ExchangeId = Model.Enums.Exchange.GDAX,
            LabelPrivateKey = "API Secret",
            LabelPublicKey = "API Key",
            LabelPassphrase = "Passphrase"
        };

        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.GDAX;
    }
}
