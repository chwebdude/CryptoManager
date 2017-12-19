using System;
using System.Collections.Generic;

namespace Plugins.Importers.Coinbase
{
    public class CoinbaseResponse<T>
    {
        public T Data { get; set; }
        public CoinbaseMessage[] Warnings { get; set; }
        public CoinbaseMessage[] Errors { get; set; }
        public CoinbasePagination Pagination { get; set; }
    }

    public class CoinbasePagination
    {
        public string Ending_Before { get; set; }
        public string Starting_After { get; set; }
        public int Limit { get; set; }
        public string Order { get; set; }
        public string Previous_Uri { get; set; }
        public string Next_Uri { get; set; }
    }

    public class CoinbaseMessage
    {
        public string Key { get; set; }
        public string Message { get; set; }
    }

    public class CoinbasePrice
    {
        public float Amount { get; set; }
    }

    public class CoinbaseWallet
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Primary { get; set; }
        public WalletType Type { get; set; }
        public CoinbaseCurrency Currency { get; set; }
        public CoinbaseBalance Balance { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public string Resource { get; set; }
        public string Resource_Path { get; set; }
    }

    public class CoinbaseBalance
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
    public class CoinbaseCurrency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int Exponent { get; set; }
        public CoinbaseCurrencyType Type { get; set; }
        public string Address_Regex { get; set; }
    }

    public enum CoinbaseCurrencyType
    {
        Fiat,
        Crypto
    }

    public enum WalletType
    {
        Fiat,
        Wallet
    }

    public class CoinbaseExchangeRate
    {
        public string Currency { get; set; }
        public Dictionary<string, float> Rates { get; set; }
    }

    public class CoinbaseTransaction
    {
        public string Id { get; set; }
        public CoinbaseTransactionTypes Type { get; set; }
        public CoinbaseTransactionStatus Status { get; set; }
        public CoinbaseBalance Amount { get; set; }
        public CoinbaseBalance Native_Amount { get; set; }
        public CoinbaseBalance Total { get; set; }
        public CoinbaseBalance Subtotal { get; set; }
        public string Description { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public DateTime Payout_At { get; set; }
        public CoinbaseBuy Buy { get; set; }
        public object From { get; set; }
        public CoinbaseTo To { get; set; }
        public CoinbaseBuy Fiat_Deposit { get; set; }
        public CoinbaseDetails Details { get; set; }
        public string Resource { get; set; }
        public string Resource_Path { get; set; }
        public bool Instant_Exchange { get; set; }
        public CoinbaseNetwork Network { get; set; }
    }

    public class CoinbaseTo
    {
        public string Address { get; set; }
        public string Currency { get; set; }
        public string Resource { get; set; }
    }

    public class CoinbaseNetwork
    {
        public string Hash { get; set; }
        public CoinbaseTransactionStatus Status { get; set; }
        public CoinbaseBalance Transaction_Amount { get; set; }
        public CoinbaseBalance Transaction_Fee { get; set; }

    }

    public class CoinbaseDetails
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Payment_Method_Name { get; set; }
    }

    public class CoinbaseBuy
    {
        public string Id { get; set; }
        public CoinbasePaymentMethod Payment_Method { get; set; }
        public CoinbaseBalance Fee { get; set; }
        public CoinbaseBalance Amount { get; set; }
        //public CoinbaseBalance Total { get; set; }
        public CoinbaseBalance Subtotal { get; set; }

        public string User_Reference { get; set; }
    }

    public class CoinbasePaymentMethod
    {
        public string Id { get; set; }
        public string Resource { get; set; }
        public string Resource_Path { get; set; }
    }

    public enum CoinbaseTransactionStatus
    {
        Pending,
        Completed,
        Canceled,
        Confirmed,
        Off_Blockchain // Ex. Gift from Coinbase
    }

    public enum CoinbaseTransactionTypes
    {
        Buy,
        Sell,
        Transfer,
        Send,
        Fiat_Deposit,
        Fiat_Withdrawal,
        Exchange_Withdrawal,
        Exchange_Deposit
    }
}
