using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CryptoManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.DTOs;

namespace CryptoManager.Controllers
{
    [Produces("application/json")]
    [Route("api/Funds")]
    public class FundsController : Controller
    {
        private readonly CryptoContext _cryptoContext;
        private readonly IMapper _mapper;

        public FundsController(CryptoContext cryptoContext, IMapper mapper)
        {
            _cryptoContext = cryptoContext;
            _mapper = mapper;
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
            }
            return fundDtos;
        }


    }
}
