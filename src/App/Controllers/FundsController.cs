using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CryptoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs;
using Plugins;

namespace CryptoManager.App.Controllers
{
    [Produces("application/json")]
    [Route("api/Funds")]
    public class FundsController : Controller
    {
        private readonly CryptoContext _cryptoContext;
        private readonly IMapper _mapper;
        private readonly IMarketData _marketData;

        public FundsController(CryptoContext cryptoContext, IMapper mapper, IMarketData marketData)
        {
            _cryptoContext = cryptoContext;
            _mapper = mapper;
            _marketData = marketData;
        }

        // GET: api/Funds
        [HttpGet]
        public async Task<IEnumerable<FundDTO>> GetAllFunds()
        {
            var funds = _cryptoContext.Funds;
            var dtos = _mapper.Map<IEnumerable<FundDTO>>(funds);
            var fundDtos = dtos as FundDTO[] ?? dtos.ToArray();
            foreach (var fundDto in fundDtos)
            {
                var exchange = await _cryptoContext.Exchanges.FindAsync(fundDto.ExchangeId);
                fundDto.ExchangeName = exchange.ExchangeId.ToString();
                fundDto.CurrentFiatRate = await _marketData.GetCurrentRate("CHF", fundDto.Currency);
                fundDto.WorthFiat = fundDto.Amount * fundDto.CurrentFiatRate;
                fundDto.CoinMeta = await _marketData.GetCoinInfo(fundDto.Currency);
            }
            return fundDtos;
        }


    }
}
