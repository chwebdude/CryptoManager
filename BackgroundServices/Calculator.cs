using System;
using System.Collections.Generic;
using System.Linq;
using CryptoManager.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Model.DbModels;
using Model.Enums;
using Plugins;

namespace BackgroundServices
{
    public class Calculator
    {
        private readonly CryptoContext _context;
        private readonly IMarketData _marketData;

        public Calculator(CryptoContext context, IMarketData marketData)
        {
            _context = context;
            _marketData = marketData;
        }

        public void RecalculateAll()
        {
            BackgroundJob.Enqueue<Calculator>(c => c.CalculateFlow());

            var transactions = _context.Transactions;
            var wallets = new Dictionary<Guid, Dictionary<string, decimal>>();
            var fiatDictionaires = new Dictionary<Guid, Dictionary<string, Fiat>>();

            foreach (var transaction in transactions)
            {
                var exchangeWallets = wallets.ContainsKey(transaction.ExchangeId) ? wallets[transaction.ExchangeId] : new Dictionary<string, decimal>();
                var fiatEx = fiatDictionaires.ContainsKey(transaction.ExchangeId)
                    ? fiatDictionaires[transaction.ExchangeId]
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
                        if (_marketData.IsFiat(transaction.InCurrency))
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
                        if (_marketData.IsFiat(transaction.OutCurrency))
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
                    default:
                        throw new NotImplementedException($"The transaction Type {transaction.Type} is not implemented");
                        break;
                }
                wallets[transaction.ExchangeId] = exchangeWallets;
                fiatDictionaires[transaction.ExchangeId] = fiatEx;
            }

            // Remove old Data
            _context.FiatBalances.RemoveRange(_context.FiatBalances);
            _context.Funds.RemoveRange(_context.Funds);
            _context.SaveChanges();

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
            foreach (var fiatDic in fiatDictionaires)
            {
                foreach (var fiat in fiatDic.Value)
                {
                    var exchangeId = fiatDic.Key;

                    var currency = fiat.Key;
                    var investment = fiat.Value.Investments;
                    var payout = fiat.Value.Payouts;
                    _context.FiatBalances.Add(new FiatBalance()
                    {
                        Currency = currency,
                        ExchangeId = exchangeId,
                        Invested = investment,
                        Payout = payout
                    });


                }
            }

            _context.SaveChanges();
        }


        public void CalculateFlow()
        {
            // Truncate old data from table
            _context.FlowNodes.RemoveRange(_context.FlowNodes);
            _context.FlowLinks.RemoveRange(_context.FlowLinks);
            _context.SaveChanges();


            // Initialize Buckets per Exchange
            var allNodes = new Dictionary<Guid, IEnumerable<FlowNode>>();
            var allLinks = new Dictionary<Guid, IEnumerable<FlowLink>>();

            foreach (var exchange in _context.Exchanges)
            {
                var links = new List<FlowLink>();

                // Add all Inputs
                var inputs =
                    _context.Transactions.Where(t => t.ExchangeId == exchange.Id && t.Type == TransactionType.In).OrderBy(t => t.DateTime);
                var nodes = inputs.Select(input => new FlowNode(input.DateTime, input.InAmount, input.InCurrency, exchange.Id, input.Id, "In")).ToList();

                // Add all Outputs
                var outputs = _context.Transactions.Where(t => t.ExchangeId == exchange.Id && t.Type == TransactionType.Out).OrderBy(t => t.DateTime);
                nodes.AddRange(outputs.Select(output => new FlowNode(output.DateTime, output.OutAmount, output.OutCurrency, exchange.Id, output.Id, "Out")));

                var transactions = _context.Transactions.Where(t => t.ExchangeId == exchange.Id).OrderBy(t => t.DateTime);

                var lastBuckets = new Dictionary<string, Guid>(); // Currency/FlowNode Id

                foreach (var transaction in transactions)
                {
                    switch (transaction.Type)
                    {
                        case TransactionType.In:
                            {
                                if (lastBuckets.ContainsKey(transaction.InCurrency))
                                {
                                    // Add Amount to new Bucket
                                    var sourceNode = nodes.Single(n => n.DateTime == transaction.DateTime && n.Amount == transaction.InAmount);

                                    var oldNode = nodes.Single(n => n.Id == lastBuckets[transaction.InCurrency]);
                                    var newNode = new FlowNode(transaction.DateTime, transaction.InAmount + oldNode.Amount, transaction.InCurrency, exchange.Id, Guid.Empty);
                                    nodes.Add(newNode);
                                    var link1 = new FlowLink(transaction.DateTime, oldNode.Amount, transaction.InCurrency, oldNode.Id, newNode.Id, exchange.Id);
                                    var link2 = new FlowLink(transaction.DateTime, transaction.InAmount, transaction.InCurrency, sourceNode.Id, newNode.Id, exchange.Id, "In");
                                    links.Add(link1);
                                    links.Add(link2);
                                    lastBuckets[transaction.InCurrency] = newNode.Id;

                                }
                                else
                                {
                                    var sourceNode = nodes.Single(n => n.TransactionId == transaction.Id);
                                    var node = new FlowNode(transaction.DateTime, transaction.InAmount, transaction.InCurrency, exchange.Id, transaction.Id);
                                    nodes.Add(node);

                                    var link = new FlowLink(transaction.DateTime, transaction.InAmount, transaction.InCurrency, sourceNode.Id, node.Id, exchange.Id, "In");
                                    links.Add(link);
                                    lastBuckets.Add(transaction.InCurrency, node.Id);
                                }
                            }

                            break;

                        case TransactionType.Trade:
                            if (lastBuckets.ContainsKey(transaction.BuyCurrency))
                            {
                                // Buy Bucket already exists
                                var buyBucketNode = nodes.Single(n => n.Id == lastBuckets[transaction.BuyCurrency]);
                                var prevNode = nodes.Single(n => n.Id == lastBuckets[transaction.SellCurrency]);
                                var restNode = new FlowNode(transaction.DateTime, prevNode.Amount - transaction.SellAmount, transaction.SellCurrency, exchange.Id, transaction.Id);
                                var newBuyBucketNode = new FlowNode(transaction.DateTime, buyBucketNode.Amount + transaction.BuyAmount, buyBucketNode.Currency, exchange.Id, transaction.Id);

                                // Calculate fee
                                if (transaction.FeeCurrency == prevNode.Currency)
                                {
                                    restNode.Amount -= transaction.FeeAmount;
                                }
                                else if (transaction.FeeCurrency == newBuyBucketNode.Currency)
                                {
                                    newBuyBucketNode.Amount -= transaction.FeeAmount;
                                }
                                else
                                {
                                    throw new NotImplementedException("Transaction Fee could not be added");
                                }

                                var linkToNewBuyBucket = new FlowLink(transaction.DateTime, transaction.BuyAmount, transaction.BuyCurrency, prevNode.Id, newBuyBucketNode.Id, exchange.Id, $"Trade {Math.Round(transaction.BuyAmount, 2)} {transaction.BuyCurrency} for {Math.Round(transaction.SellAmount, 2)} {transaction.SellCurrency}");
                                var linkToNewBucket = new FlowLink(transaction.DateTime, buyBucketNode.Amount, buyBucketNode.Currency, buyBucketNode.Id, newBuyBucketNode.Id, exchange.Id);
                                var linkToNewRestBucket = new FlowLink(transaction.DateTime, prevNode.Amount - transaction.SellAmount, transaction.SellCurrency, prevNode.Id, restNode.Id, exchange.Id);

                                // Add data
                                nodes.Add(restNode);
                                nodes.Add(newBuyBucketNode);
                                links.Add(linkToNewBuyBucket);
                                links.Add(linkToNewBucket);
                                links.Add(linkToNewRestBucket);

                                // Set new Buckets
                                lastBuckets[transaction.BuyCurrency] = newBuyBucketNode.Id;
                                lastBuckets[transaction.SellCurrency] = restNode.Id;
                            }
                            else
                            {
                                // Create new Buy Bucket
                                var prevNode = nodes.Single(n => n.Id == lastBuckets[transaction.SellCurrency]);
                                var buyNode = new FlowNode(transaction.DateTime, transaction.BuyAmount, transaction.BuyCurrency, exchange.Id, transaction.Id);
                                var sellAndRestNode = new FlowNode(transaction.DateTime, prevNode.Amount - transaction.SellAmount, transaction.SellCurrency, exchange.Id, transaction.Id);

                                // Calculate fee
                                if (transaction.FeeCurrency == prevNode.Currency)
                                {
                                    sellAndRestNode.Amount -= transaction.FeeAmount;
                                }
                                else if (transaction.FeeCurrency == buyNode.Currency)
                                {
                                    buyNode.Amount -= transaction.FeeAmount;
                                }
                                else
                                {
                                    throw new NotImplementedException("Transaction Fee could not be added");
                                }


                                // Add Links
                                var buyLink = new FlowLink(transaction.DateTime, transaction.BuyAmount, transaction.BuyCurrency, prevNode.Id, buyNode.Id, exchange.Id, $"Trade {Math.Round(transaction.BuyAmount, 2)} {transaction.BuyCurrency} for {Math.Round(transaction.SellAmount, 2)} {transaction.SellCurrency}");
                                var sellAndRestLink = new FlowLink(transaction.DateTime, prevNode.Amount - transaction.SellAmount, transaction.SellCurrency, prevNode.Id, sellAndRestNode.Id, exchange.Id);

                                // Add to Dictionaries
                                links.Add(buyLink);
                                links.Add(sellAndRestLink);
                                nodes.Add(buyNode);
                                nodes.Add(sellAndRestNode);

                                // Set new Buckets
                                lastBuckets[transaction.BuyCurrency] = buyNode.Id;
                                lastBuckets[transaction.SellCurrency] = sellAndRestNode.Id;
                            }
                            break;

                        case TransactionType.Out:
                            var previousNode = nodes.Single(n => n.Id == lastBuckets[transaction.OutCurrency]);

                            var outNode = nodes.Single(n => n.TransactionId == transaction.Id);
                            var nextNode = new FlowNode(transaction.DateTime, previousNode.Amount - outNode.Amount, transaction.OutCurrency, exchange.Id, Guid.Empty);
                            nodes.Add(nextNode);

                            var linkPreviousOut = new FlowLink(transaction.DateTime, transaction.OutAmount, transaction.OutCurrency, previousNode.Id, outNode.Id, exchange.Id, "Out");
                            var linkPreviousNext = new FlowLink(transaction.DateTime, nextNode.Amount, transaction.OutCurrency, previousNode.Id, nextNode.Id, exchange.Id);
                            links.Add(linkPreviousOut);
                            links.Add(linkPreviousNext);
                            lastBuckets[transaction.OutCurrency] = nextNode.Id;

                            break;

                        default:
                            throw new ArgumentException($"Transaction Type {transaction.Type} is unknown");
                    }
                }


                allNodes.Add(exchange.Id, nodes);
                allLinks.Add(exchange.Id, links);
            }



            // Save Data
            foreach (var allNode in allNodes)
            {
                foreach (var nodes in allNode.Value)
                {
                    _context.FlowNodes.Add(nodes);
                }
            }
            foreach (var allLink in allLinks)
            {
                foreach (var link in allLink.Value)
                {
                    _context.FlowLinks.Add(link);
                }
            }

            _context.SaveChanges();
        }

        class Fiat
        {
            public decimal Investments { get; set; }
            public decimal Payouts { get; set; }
        }

    }
}



