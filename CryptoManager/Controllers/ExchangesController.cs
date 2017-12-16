using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CryptoManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DbModels;
using Model.DTOs;
using Model.Meta;
using Plugins;

namespace CryptoManager.Controllers
{
    [Produces("application/json")]
    [Route("api/Exchanges")]
    public class ExchangesController : Controller
    {
        private readonly CryptoContext _cryptoContext;
        private readonly IMapper _mapper;


        public ExchangesController(CryptoContext cryptoContext, IMapper mapper)
        {
            _cryptoContext = cryptoContext;
            _mapper = mapper;
        }

        [HttpGet("availableExchanges")]
        public async Task<IEnumerable<ExchangeMeta>> GetAvaliableExchanges()
        {
            return await Task.Run(() =>
            {
                var type = typeof(IImporter);
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
                var res = new List<ExchangeMeta>();
                foreach (var importer in types)
                {
                    var i = (IImporter)Activator.CreateInstance(importer);
                    var meta = i.GetExchangeMeta();
                    res.Add(meta);
                }
                return res;
            });
        }

        // GET: api/Exchanges
        [HttpGet]
        public IEnumerable<ExchangeDto> Get()
        {
            var data = _cryptoContext.Exchanges.OrderByDescending(e => e.ExchangeId.ToString());
            var res = _mapper.Map<IEnumerable<ExchangeDto>>(data);
            return res;
        }


        // POST: api/Exchanges
        [HttpPost]
        public async Task AddNewImporter([FromBody]Exchange value)
        {
            // Validation
            var availableExchanges = await GetAvaliableExchanges();
            var exchangeMeta = availableExchanges.SingleOrDefault(e => e.ExchangeId == value.ExchangeId);

            if (exchangeMeta == null)
                throw new ArgumentException("No exchange Plugin exists for this exchange");

            if (exchangeMeta.SupportsPrivateKey && string.IsNullOrEmpty(value.PrivateKey))
                throw new ArgumentOutOfRangeException(nameof(value.PrivateKey), exchangeMeta.LabelPrivateKey + " was not provided");

            if (exchangeMeta.SupportsPublicKey && string.IsNullOrEmpty(value.PublicKey))
                throw new ArgumentOutOfRangeException(nameof(value.PublicKey), exchangeMeta.LabelPublicKey + " was not provided");

            // Add Data
            await _cryptoContext.Exchanges.AddAsync(value);
            await _cryptoContext.SaveChangesAsync();
        }

    }
}
