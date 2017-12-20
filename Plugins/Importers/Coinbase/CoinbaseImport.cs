using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Enums;
using Model.Meta;
using Newtonsoft.Json;
using NLog;
using RestSharp;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using Exchange = Model.DbModels.Exchange;

namespace Plugins.Importers.Coinbase
{
    public class CoinbaseImport : IImporter
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private string _apiSecret;
        private string _apiKey;
        private const string ApiVersion = "2017-08-07";

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
                    cryptoTransactions.Add(await crypto);
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
                LabelPrivateKey = "API Secret",
                LabelPublicKey = "API Key"
            };
        }

        public Model.Enums.Exchange Exchange => Model.Enums.Exchange.Coinbase;

        private static Dictionary<string, bool> IsCreditcardPaymentCache = new Dictionary<string, bool>();
        private async Task<bool> IsCreditcardPayment(string paymentMethodId)
        {
            if (IsCreditcardPaymentCache.ContainsKey(paymentMethodId))
                return IsCreditcardPaymentCache[paymentMethodId];

            var pm = await ExecuteCoinbaseGet<CoinbasePaymentMethodDetail>("/v2/payment-methods/" + paymentMethodId);
            var res = pm.Type == CoinbasePaymentMethodType.credit_card ||
                      pm.Type == CoinbasePaymentMethodType.secure3d_card;
            IsCreditcardPaymentCache.Add(paymentMethodId, res);
            return res;
        }

        private async Task<CryptoTransaction> MappTransaction(CoinbaseTransaction transaction, Guid exchangeId)
        {

            switch (transaction.Type)
            {
                case CoinbaseTransactionTypes.Buy:

                    return CryptoTransaction.NewTrade(transaction.Buy.User_Reference, transaction.Created_At, exchangeId, transaction.Details.Title + " " + transaction.Details.SubTitle,
                        transaction.Buy.Amount.Amount,
                        transaction.Buy.Amount.Currency,
                        transaction.Buy.Fee.Amount,
                        transaction.Buy.Fee.Currency,
                        transaction.Buy.Subtotal.Amount,
                        transaction.Buy.Subtotal.Currency,
                     await IsCreditcardPayment(transaction.Buy.Payment_Method.Id));


                //case CoinbaseTransactionTypes.Sell:
                //    crypto.SellAmount = transaction.Sell.Amount.Amount;
                //    crypto.SellCurrency= transaction.Sell.Amount.Currency;
                //    crypto.FeeAmount = transaction.Sell.Fee.Amount;
                //    crypto.FeeCurrency = transaction.Buy.Fee.Currency;
                //    break;

                //case CoinbaseTransactionTypes.Transfer:
                //    break;
                case CoinbaseTransactionTypes.Send:
                    // Send or receive
                    if (transaction.Network.Status == CoinbaseTransactionStatus.Off_Blockchain)
                        // Is Coinbase gift
                        return CryptoTransaction.NewIn(transaction.Id, transaction.Created_At, exchangeId,
                            transaction.Details.Title + " " + transaction.Details.SubTitle,
                            transaction.Amount.Amount,
                            transaction.Amount.Currency,
                            "Coinbase",
                            string.Empty,
                            transaction.Network.Hash);

                    if (transaction.To == null)
                    {
                        // Is Receiving
                        return CryptoTransaction.NewIn(transaction.Network.Hash, transaction.Created_At, exchangeId,
                            transaction.Details.Title + " " + transaction.Details.SubTitle,
                            transaction.Amount.Amount,
                            transaction.Amount.Currency,
                            string.Empty, // Todo: Get Network adress
                            string.Empty,
                            transaction.Network.Hash);
                    }
                    else
                    {
                        // Is Sending
                        return CryptoTransaction.NewOut(transaction.Network.Hash, transaction.Created_At, exchangeId,
                            transaction.Details.Title + " " + transaction.Details.SubTitle,
                            transaction.Network.Transaction_Amount.Amount,
                            transaction.Network.Transaction_Amount.Currency,
                            transaction.Network.Transaction_Fee.Amount,
                            transaction.Network.Transaction_Fee.Currency,
                            string.Empty,
                            transaction.To.Address,
                            transaction.Network.Hash);

                    }

                case CoinbaseTransactionTypes.Fiat_Deposit:
                    return CryptoTransaction.NewIn(transaction.Fiat_Deposit.User_Reference, transaction.Created_At, exchangeId,
                        transaction.Details.Title + " " + transaction.Details.SubTitle,
                        transaction.Fiat_Deposit.Amount.Amount,
                        transaction.Fiat_Deposit.Amount.Currency,
                        string.Empty,
                        string.Empty,
                        string.Empty);

                ////case CoinbaseTransactionTypes.Fiat_Withdrawal:
                ////    break;
                case CoinbaseTransactionTypes.Exchange_Deposit:
                    //    // Moved to GDAX
                    return CryptoTransaction.NewOut(transaction.Id, transaction.Created_At, exchangeId,
                        transaction.Details.Title + " " + transaction.Details.SubTitle,
                        -1 * transaction.Amount.Amount,
                        transaction.Amount.Currency,
                        0,
                        string.Empty,
                        string.Empty,
                        transaction.Details.SubTitle,
                        string.Empty);

                case CoinbaseTransactionTypes.Exchange_Withdrawal:
                    // From GDAX
                    return CryptoTransaction.NewIn(transaction.Id, transaction.Created_At, exchangeId,
                        transaction.Details.Title + " " + transaction.Details.SubTitle,
                        transaction.Amount.Amount,
                        transaction.Amount.Currency,
                        transaction.Details.SubTitle,
                        string.Empty,
                        string.Empty);

                default:
                    Logger.Error("Transaction Type not handled: {0}", transaction.Type);
                    throw new ArgumentOutOfRangeException();
            }
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

                var res = await client.Execute(request);
                if (res.IsSuccess)
                {
                    var content = JsonConvert.DeserializeObject<CoinbaseResponse<T>>(res.Content);

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
                else
                {
                    Logger.Error(res.StatusDescription);
                    throw new NetworkInformationException();
                }

            }
        }
    }
}
