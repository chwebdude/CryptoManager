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

        private IMarketData _marketData;

        public BinanceImport(IMarketData marketData)
        {
            this._marketData = marketData;
        }


        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            var client = new BinanceClient(exchange.PublicKey, exchange.PrivateKey);
            var trades = new List<CryptoTransaction>();
            trades.AddRange(await GetTransactions(client, exchange));
            trades.AddRange(await GetTrades(client, exchange));
            
            return trades;
        }

        private async Task<IEnumerable<CryptoTransaction>> GetTransactions(BinanceClient client, Exchange exchange)
        {
            var transactions = new List<CryptoTransaction>();
            var deposits = await client.GetDepositHistoryAsync();
            foreach (var deposit in deposits.Data.List)
            {
                transactions.Add(
                    CryptoTransaction.NewIn(deposit.InsertTime.ToFileTimeUtc().ToString(), deposit.InsertTime, exchange.Id, "Transfered "+deposit.Asset+" to Binance", deposit.Amount, deposit.Asset, string.Empty, string.Empty, string.Empty)
                    );
            }

            var withdraws = await client.GetWithdrawHistoryAsync();
            foreach (var withdrawal in withdraws.Data.List)
            {
                transactions.Add(
                    CryptoTransaction.NewOut(withdrawal.TransactionId, withdrawal.ApplyTime, exchange.Id, "Transfered "+withdrawal.Asset+ " from Binance",
                    withdrawal.Amount,
                    withdrawal.Asset,
                    0,
                    string.Empty,
                    string.Empty,
                    withdrawal.Address,
                    withdrawal.TransactionId
                    )
                    );
            }


            return transactions;
        }


        private async Task<IEnumerable<CryptoTransaction>> GetTrades(BinanceClient client, Exchange exchange)
        {
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
                                currency1, await _marketData.GetHistoricRate("CHF", currency2, trade.Time))
                        );
                    }
                    else
                    {
                        // Sell
                        trades.Add(
                        CryptoTransaction.NewTrade(trade.Id.ToString(), trade.Time, exchange.Id, "Binance Sell",
                            trade.Price * trade.Quantity, currency1, trade.Commission, trade.CommissionAsset, trade.Quantity,
                            currency2, await _marketData.GetHistoricRate("CHF", currency1, trade.Time))
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
