using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;
using Newtonsoft.Json;
using NLog;
using RestSharp;

namespace Plugins.Importers.Coinbase
{
    class CoinbaseImport : IImporter
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private string _apiSecret;
        private string _apiKey;

        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            _apiKey = exchange.PublicKey;
            _apiSecret = exchange.PrivateKey;
            var cryptoTransactions = new List<CryptoTransaction>();

            // 1. Get all Wallets
            var wallets = await ExecuteCoinbaseGet<IEnumerable<CoinbaseWallet>>("/v2/accounts");
            await Task.Delay(1000);

            // 2. Query all Wallets
            foreach (var wallet in wallets)
            {
                // 3. Query all Transactions
                Logger.Debug("Get transactions of wallet {0}", wallet.Name);
                var transactions = await ExecuteCoinbaseGet<IEnumerable<CoinbaseTransaction>>("/v2/accounts/" + wallet.Id + "/transactions?expand=all");
                foreach (var transaction in transactions)
                {
                    var crypto = MappTransaction(transaction);
                    cryptoTransactions.Add(crypto);
                }
                await Task.Delay(2000);
            }

            return cryptoTransactions;
        }

        public ExchangeMeta GetExchangeMeta()
        {
            return new ExchangeMeta()
            {
                ExchangeId = Model.Enums.Exchange.Coinbase,
                Name = "Coinbase",
                LabelPrivateKey = "Private Key",
                LabelPublicKey = "Public Key"                
            };
        }

        private CryptoTransaction MappTransaction(CoinbaseTransaction transaction)
        {
            var crypto = new CryptoTransaction
            {
                TransactionKey = transaction.Id,
                DateTime = transaction.Created_At
            };

            switch (transaction.Type)
            {
                case CoinbaseTransactionTypes.Buy:
                    crypto.BuyAmount = transaction.Buy.Amount.Amount;
                    crypto.BuyCurrency = transaction.Buy.Amount.Currency;
                    crypto.FeeAmount = transaction.Buy.Fee.Amount;
                    crypto.FeeCurrency = transaction.Buy.Fee.Currency;
                    break;

                //case CoinbaseTransactionTypes.Sell:
                //    crypto.SellAmount = transaction.Sell.Amount.Amount;
                //    crypto.SellCurrency= transaction.Sell.Amount.Currency;
                //    crypto.FeeAmount = transaction.Sell.Fee.Amount;
                //    crypto.FeeCurrency = transaction.Buy.Fee.Currency;
                //    break;

                //case CoinbaseTransactionTypes.Transfer:
                //    break;
                //case CoinbaseTransactionTypes.Send:
                //    break;
                //case CoinbaseTransactionTypes.Fiat_Deposit:
                //    break;
                //case CoinbaseTransactionTypes.Fiat_Withdrawal:
                //    break;
                //case CoinbaseTransactionTypes.Exchange_Withdrawal:
                //    break;
                //case CoinbaseTransactionTypes.Exchange_Deposit:
                //    break;
                default:
                    Logger.Error("Transaction Type not handled: {0}", transaction.Type);
                    throw new ArgumentOutOfRangeException();
            }
            return crypto;
        }

        private async Task<T> ExecuteCoinbaseGet<T>(string requestPath)
        {
            var client = new RestClient("https://api.coinbase.com/");
            var request = new RestRequest(requestPath);
            Logger.Trace("GET {0}", requestPath);
            var unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            var toSign = unixTimestamp + "GET" + requestPath;

            using (var shaAlgorithm = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret)))
            {
                var signatureBytes = Encoding.UTF8.GetBytes(toSign);
                var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
                var signatureHashHex = string.Concat(Array.ConvertAll(signatureHashBytes, b => b.ToString("X2"))).ToLower();

                request.AddHeader("CB-ACCESS-KEY", _apiKey);
                request.AddHeader("CB-ACCESS-SIGN", signatureHashHex);
                request.AddHeader("CB-ACCESS-TIMESTAMP", unixTimestamp.ToString());

                var res = await client.ExecuteTaskAsync(request);


                var content = JsonConvert.DeserializeObject<CoinbaseResponse<T>>(res.Content);

                if (content.Errors != null)
                {
                    foreach (var message in content.Errors)
                    {
                        //Todo: Log error

                    }

                }
                return content.Data;
            }
        }
    }
}
