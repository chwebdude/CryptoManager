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

namespace CryptoManager.Controllers
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

        // Todo: Filter by Exchange
        [HttpGet("Nodes")]
        public async Task<IEnumerable<FlowNodeDTO>> GetNodes()
        {
            return await Task.Run(() => _mapper.Map<IEnumerable<FlowNodeDTO>>(_context.FlowNodes));
        }

        [HttpGet("Links")]
        public async Task<IEnumerable<FlowLink>> GetLinks()
        {
            var res = await Task.Run(() => _mapper.Map<IEnumerable<FlowLinkDTO>>(_context.FlowLinks));
            return res;
        }
    }
}