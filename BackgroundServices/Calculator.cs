﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoManager.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Model.DbModels;

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
                        // Add Buy Amount
                        if (exchangeWallets.ContainsKey(transaction.BuyCurrency))
                        {
                            exchangeWallets[transaction.BuyCurrency] += transaction.BuyAmount;
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.BuyCurrency, transaction.BuyAmount);
                        }

                        // Sub Paying amount
                        if (exchangeWallets.ContainsKey(transaction.SellCurrency))
                        {
                            exchangeWallets[transaction.SellCurrency] -= transaction.SellAmount;
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.SellCurrency, -transaction.SellAmount);
                        }

                        // Sub Fee
                        if (exchangeWallets.ContainsKey(transaction.FeeCurrency))
                        {
                            exchangeWallets[transaction.FeeCurrency] -= transaction.FeeAmount;
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.FeeCurrency, -transaction.FeeAmount);
                        }


                        break;
                    case Model.Enums.TransactionType.In:
                        if (exchangeWallets.ContainsKey(transaction.InCurrency))
                        {
                            exchangeWallets[transaction.InCurrency] += transaction.InAmount;
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.InCurrency, transaction.InAmount);
                        }

                        break;
                    case Model.Enums.TransactionType.Out:
                        if (exchangeWallets.ContainsKey(transaction.OutCurrency))
                        {
                            exchangeWallets[transaction.OutCurrency] -= (transaction.OutAmount + transaction.FeeAmount);
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.OutCurrency, -(transaction.InAmount + transaction.FeeAmount));
                        }
                        break;
                }
                wallets[transaction.ExchangeId] = exchangeWallets;
            }


            // Add funds
            foreach (var wallet in wallets)
            {
                foreach (var w in wallet.Value)
                {
                    var exchangeId = wallet.Key;
                    var currency = w.Key;
                    var amount = w.Value;

                   var entity= _context.Funds.SingleOrDefault(f => f.ExchangeId == exchangeId && f.Currency == currency);
                    if (entity == null)
                    {
                        _context.Funds.Add(new Fund()
                        {
                            ExchangeId = exchangeId,
                            Currency = currency,
                            Amount = amount
                        });
                    }
                    else
                    {
                        entity.Amount = amount;
                    }
                }
            }

            _context.SaveChanges();
        }


    }
}
