using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundServices;
using CryptoManager.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DbModels;

namespace CryptoManager.Controllers
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
            return _cryptoContext.Transactions;
        }

        [HttpPost("Recalculate")]
        public IActionResult RecalculateAll()
        {
            BackgroundJob.Enqueue<Calculator>(c => c.RecalculateAll());
            return Ok();
        }
    }
}
