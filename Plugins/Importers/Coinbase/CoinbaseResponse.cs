using System;
using System.Collections.Generic;

namespace Plugins.Importers.Coinbase
{
    internal class CoinbaseResponse<T>
    {
        internal T Data { get; set; }
        internal CoinbaseMessage[] Warnings { get; set; }
        internal CoinbaseMessage[] Errors { get; set; }
    }

    internal class CoinbaseMessage
    {
        internal string Key { get; set; }
        internal string Message { get; set; }
    }

    internal class CoinbasePrice
    {
        internal float Amount { get; set; }
    }

    internal class CoinbaseWallet
    {
        internal string Id { get; set; }
        internal string Name { get; set; }
        internal bool Primary { get; set; }
        internal WalletType Type { get; set; }
        internal CoinbaseCurrency Currency { get; set; }
        internal CoinbaseBalance Balance { get; set; }
    }

    internal class CoinbaseBalance
    {
        internal decimal Amount { get; set; }
        internal string Currency { get; set; }
    }
    internal class CoinbaseCurrency
    {
        internal string Code { get; set; }
        internal string Name { get; set; }
    }

    internal enum WalletType
    {
        Fiat,
        Wallet
    }

    internal class CoinbaseExchangeRate
    {
        internal string Currency { get; set; }
        internal Dictionary<string, float> Rates { get; set; }
    }

    internal class CoinbaseTransaction
    {
        internal string Id { get; set; }
        internal CoinbaseTransactionTypes Type { get; set; }
        internal CoinbaseTransactionStatus Status { get; set; }
        internal CoinbaseBalance Amount { get; set; }
        internal CoinbaseBalance Native_Amount { get; set; }
        //internal CoinbaseBalance Total { get; set; }
        //internal CoinbaseBalance Subtotal { get; set; }
        //internal string Description { get; set; }
        internal DateTime Created_At { get; set; }
        internal DateTime Updated_At { get; set; }
        internal DateTime Payout_At { get; set; }
        internal CoinbaseBuy Buy { get; set; }
        internal object From { get; set; }
        internal object To { get; set; }
        internal CoinbaseBuy Fiat_Deposit { get; set; }
        internal CoinbaseDetails Details { get; set; }
       
    }

    internal class CoinbaseDetails
    {
        internal string Title { get; set; }
        internal string SubTitle { get; set; }
        internal string Payment_Method_Name { get; set; }
    }

    internal class CoinbaseBuy
    {
        //internal CoinbasePaymentMethod Payment_Method { get; set; }
        internal CoinbaseBalance Fee { get; set; }
        internal CoinbaseBalance Amount { get; set; }
        //internal CoinbaseBalance Total { get; set; }
        internal CoinbaseBalance Subtotal { get; set; }
    }

    internal class CoinbasePaymentMethod
    {
        internal string Id { get; set; }
        internal string Resource { get; set; }
        internal string Resource_Path { get; set; }
    }

    internal enum CoinbaseTransactionStatus
    {
        Pending,
        Completed,
        Canceled
    }

    internal enum CoinbaseTransactionTypes
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
