using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CryptoManager.Models;
using Microsoft.AspNetCore.Mvc;
using Model.DbModels;
using Model.DTOs;

namespace CryptoManager.App.Controllers
{
    [Produces("application/json")]
    [Route("api/Flows")]
    public class FlowsController : Controller
    {
        private readonly CryptoContext _context;
        private readonly IMapper _mapper;

        public FlowsController(CryptoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Nodes")]
        public async Task<IEnumerable<FlowNodeDTO>> GetNodes(Guid exchangeId)
        {
            return await Task.Run(() => _mapper.Map<IEnumerable<FlowNodeDTO>>(_context.FlowNodes.Where(f => f.ExchangeId == exchangeId)));
        }

        [HttpGet("Links")]
        public async Task<IEnumerable<FlowLink>> GetLinks(Guid exchangeId)
        {
            var res = await Task.Run(() => _mapper.Map<IEnumerable<FlowLinkDTO>>(_context.FlowLinks.Where(f => f.ExchangeId == exchangeId)));
            return res;
        }
    }
}