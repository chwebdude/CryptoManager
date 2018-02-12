using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Meta;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Plugins.MarketData;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace Plugins.Importers.Bity
{
    public class BityImport : IImporter
    {
        private readonly IMarketData _marketData;

        public BityImport(IMarketData marketData)
        {
            _marketData = marketData;
        }

        public async Task<IEnumerable<CryptoTransaction>> GetTransactions(Exchange exchange)
        {
            var client = new RestClient("https://bity.com/");


            var authRequest = new RestRequest("o/token/", Method.POST);
            authRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            authRequest.AddHeader("Accept", "application/json, text/plain, */*");
            authRequest.AddParameter("client_id", "QmaTkYI50XmCF18fupZgdAOptYqDzVix12RpqFYS");
            authRequest.AddParameter("grant_type", "password");
            authRequest.AddParameter("username", exchange.PublicKey);
            authRequest.AddParameter("password", exchange.Passphrase);

            var authResp = await client.Execute(authRequest);
            var token = JsonConvert.DeserializeObject<BityAuthResponse>(authResp.Content);


            var historyRequest = new RestRequest("/api/v1/order/?limit=100&offset=0&order_by=-timestamp_created", Method.GET);
            historyRequest.AddHeader("Authorization", token.Token_Type + " " + token.Access_Token);
            var historyResponse = await client.Execute(historyRequest);


            var data = JsonConvert.DeserializeObject<Orders>(historyResponse.Content);
            var executed = data.Objects.Where(o => o.Status == "EXEC");

            var transactions = new List<CryptoTransaction>();
            foreach (var trade in executed)
            {
                var inputCurrency = trade.Inputtransactions.First().Currency;
                var inputAmount = trade.Inputtransactions.Sum(i => i.Amount);
                var outputCurrency = trade.Outputtransactions.First().Currency;
                var outputAmount = trade.Outputtransactions.Sum(o => o.Amount);
                var fee = trade.Outputtransactions.Sum(o => o.PaymentProcessorFee) +
                          trade.Inputtransactions.Sum(i => i.PaymentProcessorFee);
                var date = trade.TimestampCreated;
                var inputReference = trade.Inputtransactions.First().Reference;
                var outputReference = trade.Outputtransactions.First().Reference;
                var fiatRate = await _marketData.GetHistoricRate("CHF", outputCurrency, date);

                // Input Transaction
                transactions.Add(CryptoTransaction.NewIn(
                    inputReference,
                    date,
                    exchange.Id,
                    "Receving",
                    inputAmount,
                    inputCurrency,
                    inputReference,
                    string.Empty,
                    string.Empty
                    ));

                // Trade
                transactions.Add(CryptoTransaction.NewTrade(
                    inputReference + " to " + outputReference,
                    date,
                    exchange.Id,
                    "Trade", outputAmount,
                    outputCurrency,
                    (decimal) fee,
                    outputCurrency,
                    inputAmount,
                    inputCurrency,
                    fiatRate
                    ));

                // Output Transaction
                transactions.Add(CryptoTransaction.NewOut(
                    outputReference,
                    date,
                    exchange.Id,
                    "Sending",
                    outputAmount,
                    outputCurrency,
                    (decimal) fee,
                    outputCurrency,
                    string.Empty,
                    string.Empty,
                    outputReference
                    ));
            }

            return transactions;
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
