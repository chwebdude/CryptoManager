using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.DbModels;

namespace CryptoManager.Controllers
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
        public IEnumerable<FiatBalance> Get()
        {
            return _cryptoContext.FiatBalances;
        }        
    }
}
