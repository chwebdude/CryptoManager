using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Binance.Net;
using Binance.Net.Objects;
using Model.DbModels;
using Model.Meta;
using Newtonsoft.Json;
using NLog;

namespace Plugins.Importers.Binance
{
    public class BinanceImport : IImporter
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {


            var client = new BinanceClient(exchange.PublicKey, exchange.PrivateKey);
            var trades = new List<CryptoTransaction>();

            var prices = await client.GetAllPricesAsync();
            var counter = 0;

            foreach (var binancePrice in prices.Data)
            {
                var symbol = binancePrice.Symbol;
                Logger.Debug("Ask " + symbol + " (" + counter++ + "/" + prices.Data.Length + ")");
                string currency1 = null, currency2 = null;
                var bccTrades = await client.GetMyTradesAsync(symbol);

                if (symbol.EndsWith("BTC"))
                {
                    // Is BTC
                    currency1 = "BTC";
                    currency2 = symbol.Replace("BTC", "");
                }

                if (symbol.EndsWith("ETH"))
                {
                    // Is ETH
                    currency1 = "ETH";
                    currency2 = symbol.Replace("ETH", "");
                }

                if (symbol.EndsWith("BNB"))
                {
                    // Is BNB
                    currency1 = "BNB";
                    currency2 = symbol.Replace("BNB", "");
                }

                if (symbol.EndsWith("USDT"))
                {
                    // Is USDT
                    currency1 = "USDT";
                    currency2 = symbol.Replace("USDT", "");
                }

                if (currency1 == null || currency2 == null)
                {
                    if (symbol == "123456")
                    {
                        Logger.Warn("{0} symbol is not supported", symbol);
                        continue;
                    }

                    Logger.Error("Unknown symbol: {0}. Aborting!", symbol);

                    throw new ArgumentOutOfRangeException("Unknown symbol: " + symbol);
                }

                foreach (var trade in bccTrades.Data)
                {
                    if (trade.IsBuyer)
                    {
                        // Buy
                        trades.Add(
                            CryptoTransaction.NewTrade(trade.Id.ToString(), trade.Time, exchange.Id, "Binance Buy",
                                trade.Quantity, currency2, trade.Commission, trade.CommissionAsset,
                                trade.Price * trade.Quantity,
                                currency1, true)
                        );
                    }
                    else
                    {
                        // Sell
                        trades.Add(
                        CryptoTransaction.NewTrade(trade.Id.ToString(), trade.Time, exchange.Id, "Binance Sell",
                            trade.Price * trade.Quantity, currency1, trade.Commission, trade.CommissionAsset, trade.Quantity,
                            currency2, true)
                            );
                    }
                }
            }



            return trades;
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
