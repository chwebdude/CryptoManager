using System;
using System.Collections.Generic;
using System.Linq;
using CryptoManager.Models;
using Hangfire;
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
            BackgroundJob.Enqueue<Calculator>(c => c.CalculateFlow());

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
            // Truncate old data from table
            _context.FlowNodes.RemoveRange(_context.FlowNodes);
            _context.FlowLinks.RemoveRange(_context.FlowLinks);
            _context.SaveChanges();


            // Initialize Buckets per Exchange
            var allNodes = new Dictionary<Guid, IEnumerable<FlowNode>>();
            var allLinks = new Dictionary<Guid, IEnumerable<FlowLink>>();

            foreach (var exchange in _context.Exchanges)
            {
                var nodes = new List<FlowNode>();
                var links = new List<FlowLink>();

                // Add all Inputs
                var inputs =
                    _context.Transactions.Where(t => t.ExchangeId == exchange.Id && t.Type == TransactionType.In).OrderBy(t => t.DateTime);
                foreach (var input in inputs)
                {
                    nodes.Add(new FlowNode(input.DateTime, input.InAmount, input.InCurrency, exchange.Id, input.Id, "In"));
                }

                // Add all Outputs
                var outputs = _context.Transactions.Where(t => t.ExchangeId == exchange.Id && t.Type == TransactionType.Out).OrderBy(t => t.DateTime);
                foreach (var output in outputs)
                {
                    nodes.Add(new FlowNode(output.DateTime, output.OutAmount, output.OutCurrency, exchange.Id, output.Id, "Out"));
                }

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
                                    var link1 = new FlowLink(transaction.DateTime, oldNode.Amount, transaction.InCurrency, oldNode.Id, newNode.Id);
                                    var link2 = new FlowLink(transaction.DateTime, transaction.InAmount, transaction.InCurrency, sourceNode.Id, newNode.Id, "In");
                                    links.Add(link1);
                                    links.Add(link2);
                                    lastBuckets[transaction.InCurrency] = newNode.Id;

                                }
                                else
                                {
                                    var sourceNode = nodes.Single(n => n.TransactionId == transaction.Id);
                                    var node = new FlowNode(transaction.DateTime, transaction.InAmount, transaction.InCurrency, exchange.Id, transaction.Id);
                                    nodes.Add(node);

                                    var link = new FlowLink(transaction.DateTime, transaction.InAmount, transaction.InCurrency, sourceNode.Id, node.Id, "In");
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
                                nodes.Add(restNode);
                                nodes.Add(newBuyBucketNode);

                                var linkToNewBuyBucket = new FlowLink(transaction.DateTime, transaction.BuyAmount, transaction.BuyCurrency, prevNode.Id, newBuyBucketNode.Id, "Buy");
                                var linkToNewBucket = new FlowLink(transaction.DateTime, buyBucketNode.Amount, buyBucketNode.Currency, buyBucketNode.Id, newBuyBucketNode.Id);
                                var linkToNewRestBucket = new FlowLink(transaction.DateTime, prevNode.Amount - transaction.SellAmount, transaction.SellCurrency, prevNode.Id, restNode.Id);

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
                                nodes.Add(buyNode);
                                nodes.Add(sellAndRestNode);


                                // Add Links
                                var buyLink = new FlowLink(transaction.DateTime, transaction.BuyAmount, transaction.BuyCurrency, prevNode.Id, buyNode.Id, "Buy");
                                var sellAndRestLink = new FlowLink(transaction.DateTime, prevNode.Amount - transaction.SellAmount, transaction.SellCurrency, prevNode.Id, sellAndRestNode.Id);
                                links.Add(buyLink);
                                links.Add(sellAndRestLink);

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

                            var linkPreviousOut = new FlowLink(transaction.DateTime, transaction.OutAmount, transaction.OutCurrency, previousNode.Id, outNode.Id, "Out");
                            var linkPreviousNext = new FlowLink(transaction.DateTime, nextNode.Amount, transaction.OutCurrency, previousNode.Id, nextNode.Id);
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













            //foreach (var transaction in transactions)
            //{
            //    var flow = new FlowNode()
            //    {
            //        TransactionId = transaction.Id,
            //        DateTime = transaction.DateTime,
            //    };
            //    switch (transaction.Type)
            //    {
            //        case TransactionType.Trade:
            //            break;
            //        case TransactionType.In:
            //            // Search for Sender 
            //            var senderTransaction =
            //                _context.Transactions.SingleOrDefault(t => !string.IsNullOrEmpty(t.TransactionHash) && t.TransactionHash == transaction.TransactionHash && t.Id != transaction.Id);
            //            flow.Amount = transaction.InAmount;
            //            flow.Currency = transaction.InCurrency;
            //            flow.ExchangeId = transaction.ExchangeId;

            //            if (senderTransaction != null)
            //            {
            //                var parentFlow = _context.Flows.Single(f => f.TransactionId == senderTransaction.Id);
            //                flow.Parents = new List<FlowNode>() { parentFlow };
            //            }

            //            break;
            //        case TransactionType.Out:
            //            flow.Amount = transaction.OutAmount;
            //            flow.Currency = transaction.OutCurrency;

            //            //Todo: Get last from time
            //            var lastFlow = _context.Flows.First(f =>
            //                f.Currency == flow.Currency && f.ExchangeId == transaction.ExchangeId);
            //            flow.Parents = new List<FlowNode>() { lastFlow };

            //            //Todo: Determine Exchange
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException();
            //    }

            //    _context.Flows.Add(flow);

            //}

            //var start = transactions.First();
            //_context.Flows.Add(new FlowNode
            //{
            //    DateTime = start.DateTime,
            //    Amount = start.BuyAmount,
            //    Currency = start.BuyCurrency
            //});

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



