using System.Collections.Generic;
using CryptoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs;

namespace CryptoManager.App.Controllers
{
    [Produces("application/json")]
    [Route("api/FiatBalances")]
    public class FiatBalancesController : Controller
    {
        private readonly CryptoContext _cryptoContext;

        public FiatBalancesController(CryptoContext cryptoContext)
        {
            _cryptoContext = cryptoContext;
        }

        // GET: api/FiatBalances
        [HttpGet]
        public IEnumerable<FiatDTO> Get()
        {
            foreach (var cryptoContextFiatBalance in _cryptoContext.FiatBalances)
            {
                var exchange = _cryptoContext.Exchanges.Find(cryptoContextFiatBalance.ExchangeId);
                yield return new FiatDTO()
                {

                    ExchangeId = cryptoContextFiatBalance.ExchangeId,
                    Currency = cryptoContextFiatBalance.Currency,
                    ExchangeName = exchange.ExchangeId.ToString(),
                    Id = cryptoContextFiatBalance.Id,
                    Invested = cryptoContextFiatBalance.Invested,
                    Payout = cryptoContextFiatBalance.Payout
                };
            }

        }
    }
}
