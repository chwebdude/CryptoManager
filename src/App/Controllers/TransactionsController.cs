using System.Collections.Generic;
using System.Linq;
using BackgroundServices;
using CryptoManager.Models;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Model.DbModels;

namespace CryptoManager.App.Controllers
{
    [Produces("application/json")]
    [Route("api/Transactions")]
    public class TransactionsController : Controller
    {
        private readonly CryptoContext _cryptoContext;

        public TransactionsController(CryptoContext cryptoContext)
        {
            _cryptoContext = cryptoContext;
        }

        // GET: api/Transactions
        [HttpGet]
        public IEnumerable<CryptoTransaction> Get()
        {
            return _cryptoContext.Transactions.OrderByDescending(t => t.DateTime); ;
        }

        [HttpPost("Recalculate")]
        public IActionResult RecalculateAll()
        {
            BackgroundJob.Enqueue<Calculator>(c => c.RecalculateAll());
            return Ok();
        }
    }
}
