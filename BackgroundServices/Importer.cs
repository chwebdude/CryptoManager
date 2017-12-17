using CryptoManager.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Model.DbModels;
using Model.Enums;
using Plugins;
using Exchange = Model.Enums.Exchange;

namespace BackgroundServices
{
    public class Importer
    {
        private readonly CryptoContext _context;

        public Importer(CryptoContext context)
        {
            _context = context;
        }

        public async Task Import(Guid exchangeId)
        {
            var exchange = _context.Exchanges.Find(exchangeId);
            var importer = GetImporter(exchange.ExchangeId);
            var transactions = await importer.GetTransactions(exchange);


            //_context.Transactions.Add(new CryptoTransaction()
            //{
            //    Type = TransactionType.In,
            //    ExchangeId = new Guid(),
            //    BuyAmount = 22,
            //    Id = new Guid()
            //});
            //_context.SaveChanges();
        }


        public async Task ImportAll()
        {
            var exchanges = _context.Exchanges;
            foreach (var exchange in exchanges)
            {
                await Import(exchange.Id);
            }
        }

        private IImporter GetImporter(Exchange exchangeId)
        {
            var type = typeof(IImporter);
            var allImporters = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
            foreach (var allImporter in allImporters)
            {

                var i = (IImporter)Activator.CreateInstance(allImporter);
                if (i.Exchange == exchangeId)
                    return i;
            }
            throw new ApplicationException("No plugin for exchange " + exchangeId + " found");
        }

    }
}
