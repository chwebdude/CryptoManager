using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Enums;
using Model.Meta;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using Exchange = Model.DbModels.Exchange;

namespace Plugins.Importers.Coinbase
{
    public class CoinbaseImport : IImporter
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private string _apiSecret;
        private string _apiKey;
        private const string ApiVersion = "2017-12-18";

        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            _apiKey = exchange.PublicKey;
            _apiSecret = exchange.PrivateKey;
            var cryptoTransactions = new List<CryptoTransaction>();

            // 1. Get all Wallets
            //var wallets = await ExecuteCoinbaseGet<CoinbaseWallet[]>("/v2/accounts");
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
                    var crypto = MappTransaction(transaction, exchange.Id);
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
                ExchangeId = Exchange,
                Name = "Coinbase",
                LabelPrivateKey = "Private Key",
                LabelPublicKey = "Public Key"
            };
        }

        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.Coinbase;

        private CryptoTransaction MappTransaction(CoinbaseTransaction transaction, Guid exchangeId)
        {
            var crypto = new CryptoTransaction
            {
                TransactionKey = transaction.Id,
                DateTime = transaction.Created_At,
                ExchangeId = exchangeId,
                Comment = transaction.Details.Title + " " + transaction.Details.SubTitle
            };

            switch (transaction.Type)
            {
                case CoinbaseTransactionTypes.Buy:
                    crypto.BuyAmount = transaction.Buy.Amount.Amount;
                    crypto.BuyCurrency = transaction.Buy.Amount.Currency;
                    crypto.FeeAmount = transaction.Buy.Fee.Amount;
                    crypto.FeeCurrency = transaction.Buy.Fee.Currency;
                    crypto.Rate = transaction.Buy.Subtotal.Amount / transaction.Buy.Amount.Amount;
                    crypto.Type = TransactionType.Trade;
                    break;

                //case CoinbaseTransactionTypes.Sell:
                //    crypto.SellAmount = transaction.Sell.Amount.Amount;
                //    crypto.SellCurrency= transaction.Sell.Amount.Currency;
                //    crypto.FeeAmount = transaction.Sell.Fee.Amount;
                //    crypto.FeeCurrency = transaction.Buy.Fee.Currency;
                //    break;

                //case CoinbaseTransactionTypes.Transfer:
                //    break;
                case CoinbaseTransactionTypes.Send:
                    // Receiving
                    crypto.InAmount = transaction.Amount.Amount;
                    crypto.InCurrency = transaction.Amount.Currency;
                    crypto.TransactionHash = transaction.Network.Hash;
                    break;
                case CoinbaseTransactionTypes.Fiat_Deposit:
                    crypto.Type = TransactionType.In;
                    crypto.InAmount = transaction.Fiat_Deposit.Amount.Amount;
                    crypto.InCurrency = transaction.Fiat_Deposit.Amount.Currency;
                    break;
                //case CoinbaseTransactionTypes.Fiat_Withdrawal:
                //    break;
                case CoinbaseTransactionTypes.Exchange_Deposit:
                    // Moved to GDAX
                    crypto.Type = TransactionType.Transfer;
                    crypto.OutAmount = -1 * transaction.Amount.Amount;
                    crypto.OutCurrency = transaction.Amount.Currency;
                    break;
                case CoinbaseTransactionTypes.Exchange_Withdrawal:
                    // From GDAX
                    crypto.Type = TransactionType.Transfer;
                    crypto.InAmount = transaction.Amount.Amount;
                    crypto.InCurrency = transaction.Amount.Currency;
                    break;
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
                request.AddHeader("CB-VERSION", ApiVersion);

                var res = await client.ExecuteTaskAsync(request);

                //var settings = new JsonSerializerSettings()
                //{
                //    NullValueHandling = NullValueHandling.Ignore,
                //    MissingMemberHandling = MissingMemberHandling.Error,                    
                //};
                var content = JsonConvert.DeserializeObject<CoinbaseResponse<T>>(res.Content/*, settings*/);

                if (content.Errors != null)
                {
                    foreach (var message in content.Errors)
                    {
                        //Todo: Log error
                        Logger.Error(message);
                    }

                }
                return content.Data;
            }
        }



    }
}
