﻿using System;
using System.Collections.Generic;
using System.Linq;
using CryptoManager.Models;
using Microsoft.EntityFrameworkCore;
using Model.DbModels;
using Model.Enums;

namespace BackgroundServices
{
    public class Calculator
    {
        private readonly CryptoContext _context;
        private static readonly string[] FiatCurrencies = { "EUR", "CHF", "USD" };

        public Calculator(CryptoContext context)
        {
            _context = context;
        }

        public void RecalculateAll()
        {
            var transactions = _context.Transactions;
            var wallets = new Dictionary<Guid, Dictionary<string, decimal>>();
            //var fiatInvestments = new Dictionary<Guid, Dictionary<string, decimal>>();
            //var fiatPayouts = new Dictionary<Guid, Dictionary<string, decimal>>();
            var fiatDict = new Dictionary<Guid, Dictionary<string, Fiat>>();

            foreach (var transaction in transactions)
            {
                var exchangeWallets = wallets.ContainsKey(transaction.ExchangeId) ? wallets[transaction.ExchangeId] : new Dictionary<string, decimal>();
                var fiatEx = fiatDict.ContainsKey(transaction.ExchangeId)
                    ? fiatDict[transaction.ExchangeId]
                    : new Dictionary<string, Fiat>();


                switch (transaction.Type)
                {
                    case TransactionType.Trade:
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


                    case TransactionType.In:
                        if (exchangeWallets.ContainsKey(transaction.InCurrency))
                        {
                            exchangeWallets[transaction.InCurrency] += transaction.InAmount;
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.InCurrency, transaction.InAmount);
                        }

                        // Is this a FiatBalance 
                        if (FiatCurrencies.Contains(transaction.InCurrency))
                        {
                            if (fiatEx.ContainsKey(transaction.InCurrency))
                            {
                                fiatEx[transaction.InCurrency].Investments += transaction.InAmount;
                            }
                            else
                            {
                                fiatEx.Add(transaction.InCurrency, new Fiat
                                {
                                    Investments = transaction.InAmount
                                });
                            }
                        }

                        break;
                    case TransactionType.Out:
                        if (exchangeWallets.ContainsKey(transaction.OutCurrency))
                        {
                            exchangeWallets[transaction.OutCurrency] -= (transaction.OutAmount + transaction.FeeAmount);
                        }
                        else
                        {
                            exchangeWallets.Add(transaction.OutCurrency, -(transaction.OutAmount + transaction.FeeAmount));
                        }

                        // Is this a FiatBalance 
                        if (FiatCurrencies.Contains(transaction.OutCurrency))
                        {
                            if (fiatEx.ContainsKey(transaction.OutCurrency))
                            {
                                fiatEx[transaction.OutCurrency].Payouts += transaction.OutAmount;
                            }
                            else
                            {
                                fiatEx.Add(transaction.OutCurrency, new Fiat()
                                {
                                    Payouts = transaction.OutAmount
                                });
                            }
                        }

                        break;
                }
                wallets[transaction.ExchangeId] = exchangeWallets;
                fiatDict[transaction.ExchangeId] = fiatEx;
            }


            // Add funds
            foreach (var wallet in wallets)
            {
                foreach (var w in wallet.Value)
                {
                    var exchangeId = wallet.Key;
                    var currency = w.Key;
                    var amount = w.Value;

                    var entity = _context.Funds.SingleOrDefault(f => f.ExchangeId == exchangeId && f.Currency == currency);
                    if (entity == null)
                    {
                        _context.Funds.Add(new Fund
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

            // Add Fiat Balances
            foreach (var fiatDic in fiatDict)
            {
                foreach (var fiatDictValue in fiatDict.Values)
                {
                    var exchangeId = fiatDic.Key;
                    foreach (var fiat in fiatDictValue)
                    {
                        var currency = fiat.Key;
                        var investment = fiat.Value.Investments;
                        var payout = fiat.Value.Payouts;

                        var entity = _context.FiatBalances.SingleOrDefault(f =>
                            f.ExchangeId == exchangeId && f.Currency == currency);
                        if (entity == null)
                        {
                            _context.FiatBalances.Add(new FiatBalance()
                            {
                                Currency = currency,
                                ExchangeId = exchangeId,
                                Invested = investment,
                                Payout = payout
                            });
                        }
                        else
                        {
                            entity.Invested = investment;
                            entity.Payout = payout;
                        }
                    }

                }
            }

            _context.SaveChanges();
        }


        public void CalculateFlow()
        {
            var transactions = _context.Transactions.OrderBy(t => t.DateTime);
            _context.Flows.RemoveRange(_context.Flows);
            _context.SaveChanges();


            foreach (var transaction in transactions)
            {
                var flow = new Flow()
                {
                    TransactionId = transaction.Id,
                    DateTime = transaction.DateTime,
                };
                switch (transaction.Type)
                {
                    case TransactionType.Trade:
                        break;
                    case TransactionType.In:
                        // Search for Sender 
                        var senderTransaction =
                            _context.Transactions.SingleOrDefault(t => !string.IsNullOrEmpty(t.TransactionHash) && t.TransactionHash == transaction.TransactionHash && t.Id != transaction.Id);
                        flow.Amount = transaction.InAmount;
                        flow.Currency = transaction.InCurrency;
                        flow.ExchangeId = transaction.ExchangeId;

                        if (senderTransaction != null)
                        {
                            var parentFlow = _context.Flows.Single(f => f.TransactionId == senderTransaction.Id);
                            flow.Parents = new List<Flow>() { parentFlow };
                        }

                        break;
                    case TransactionType.Out:
                        flow.Amount = transaction.OutAmount;
                        flow.Currency = transaction.OutCurrency;

                        //Todo: Get last from time
                        var lastFlow = _context.Flows.First(f =>
                            f.Currency == flow.Currency && f.ExchangeId == transaction.ExchangeId);
                        flow.Parents = new List<Flow>() { lastFlow };

                        //Todo: Determine Exchange
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _context.Flows.Add(flow);

            }

            var start = transactions.First();
            _context.Flows.Add(new Flow
            {
                DateTime = start.DateTime,
                Amount = start.BuyAmount,
                Currency = start.BuyCurrency
            });
            _context.SaveChanges();
        }

        class Fiat
        {
            public decimal Investments { get; set; }
            public decimal Payouts { get; set; }
        }

    }
}
