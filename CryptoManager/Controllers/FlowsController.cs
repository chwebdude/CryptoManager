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
    [Route("api/Flows")]
    public class FlowsController : Controller
    {
        private readonly CryptoContext _context;

        public FlowsController(CryptoContext context)
        {
            _context = context;
        }

        // Todo: Filter by Exchange
        [HttpGet]
        public async Task<IEnumerable<FlowNode>> GetNodes()
        {
            return await Task.Run(() => _context.FlowNodes);            
        }
    }
}