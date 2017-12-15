using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.Meta;
using Plugins;

namespace CryptoManager.Controllers
{
    [Produces("application/json")]
    [Route("api/Exchanges")]
    public class ExchangesController : Controller
    {
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
                    var i = (IImporter) Activator.CreateInstance(importer);
                    var meta = i.GetExchangeMeta();
                    res.Add(meta);
                }
                return res;
            });
        }

        // GET: api/Exchanges
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Exchanges/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Exchanges
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Exchanges/5
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
