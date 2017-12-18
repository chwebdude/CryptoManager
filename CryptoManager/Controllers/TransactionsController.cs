using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DbModels;

namespace CryptoManager.Controllers
{
    [Produces("application/json")]
    [Route("api/Transactions")]
    public class TransactionsController : Controller
    {
        private CryptoContext _cryptoContext;

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
      
    }
}
