using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CryptoManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs;
using Model.Enums;
using Plugins;

namespace CryptoManager.Controllers
{
    [Produces("application/json")]
    [Route("api/Investments")]
    public class InvestmentsController : Controller
    {
        private CryptoContext _cryptoContext;
        private IMarketData _marketData;
        private readonly IMapper _mapper;


        public InvestmentsController(CryptoContext cryptoContext, IMarketData marketData, IMapper mapper)
        {
            _cryptoContext = cryptoContext;
            _marketData = marketData;
            _mapper = mapper;
        }

        // GET: api/Investments
        [HttpGet]
        public async Task<IEnumerable<InvestmentDTO>> Get()
        {
            var trades = _cryptoContext.Transactions.Where(t => t.Type == TransactionType.Trade);
            var res = new List<InvestmentDTO>();
            foreach (var trade in trades)
            {
                var currentRate = await _marketData.GetCurrentRate("CHF", trade.BuyCurrency);
                var dto = _mapper.Map<InvestmentDTO>(trade);
                dto.CurrentFiatRate = currentRate;
                dto.CurrentFiatValue = dto.BuyAmount * currentRate;
                res.Add(dto);
            }
            return res;
        }

        // GET: api/Investments/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Investments
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Investments/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
