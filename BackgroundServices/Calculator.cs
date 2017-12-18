using System;
using System.Collections.Generic;
using System.Text;
using CryptoManager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BackgroundServices
{
    public class Calculator
    {
        private readonly CryptoContext _context;

        public Calculator(CryptoContext context)
        {
            _context = context;
        }

        public void RecalculateAll()
        {
            var transactions = _context.Transactions;
            var wallets = new Dictionary<Guid, Dictionary<string, decimal>>();

            foreach (var transaction in transactions)
            {
                var exchangeWallets = wallets.ContainsKey(transaction.ExchangeId) ? wallets[transaction.ExchangeId] : new Dictionary<string, decimal>();


                switch (transaction.Type)
                {
                    case Model.Enums.TransactionType.Trade:
                        if (exchangeWallets.ContainsKey(transaction.BuyCurrency))
                        {
                            exchangeWallets[transaction.BuyCurrency] += transaction.BuyAmount;
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.BuyCurrency, transaction.BuyAmount);
                        }

                        break;
                    case Model.Enums.TransactionType.In:
                        break;
                    case Model.Enums.TransactionType.Out:
                        break;
                }
                wallets[transaction.ExchangeId] = exchangeWallets;
            }
        }


    }
}
