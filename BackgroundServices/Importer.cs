using CryptoManager.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Model.DbModels;
using Model.Enums;
using Plugins;
using Exchange = Model.Enums.Exchange;

namespace BackgroundServices
{
    public class Importer
    {
        private readonly CryptoContext _context;
        private IMarketData _marketData;

        public Importer(CryptoContext context, IMarketData marketData)
        {
            _context = context;
            _marketData = marketData;
        }

        public async Task Import(Guid exchangeId, bool recalculate)
        {
            var exchange = _context.Exchanges.Find(exchangeId);
            var importer = GetImporter(exchange.ExchangeId);
            var transactions = await importer.GetTransactionsAsync(exchange);

            foreach (var transaction in transactions)
            {
                // Check if already in database and skip
                var row = _context.Transactions.SingleOrDefault(t =>
                    t.ExchangeId == exchangeId && t.TransactionKey == transaction.TransactionKey);
                if (row == null)
                {
                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();
                }
            }

            if (recalculate)
                BackgroundJob.Enqueue<Calculator>(c => c.RecalculateAll());
        }


        public async Task ImportAll()
        {
            var exchanges = _context.Exchanges;
            foreach (var exchange in exchanges)
            {
                await Import(exchange.Id, false);
            }

            BackgroundJob.Enqueue<Calculator>(c => c.RecalculateAll());

        }

        private IImporter GetImporter(Exchange exchangeId)
        {
            var type = typeof(IImporter);
            var allImporters = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
            foreach (var allImporter in allImporters)
            {

                var i = (IImporter)Activator.CreateInstance(allImporter, _marketData);
                if (i.Exchange == exchangeId)
                    return i;
            }
            throw new ApplicationException("No plugin for exchange " + exchangeId + " found");
        }

    }
}
