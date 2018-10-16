using ExchangeWatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MarketObserver.Controllers
{
    [Route("api/")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        [Route("api/addcotacoes")]
        public string Get()
        {
            DataController dt = new DataController();
            return dt.AddData();
        }

        // GET api/cotacoes
        [HttpGet]
        [Route("api/cotacoes")]
        public string GetCotacao()
        {
            DataController dt = new DataController();
            return dt.GetCotacoes();
        }

        [HttpGet]
        [Route("api/calculos")]
        public string GetCalculos()
        {
            DataController dt = new DataController();
            return dt.GetCalculos();
        }

        [HttpGet]
        [Route("api/addusd")]
        public string AddUsdValues()
        {
            DataController dt = new DataController();
            return dt.AddUsdData();
        }

        [HttpGet]
        [Route("api/getusd")]
        public string GetUsdValues()
        {
            DataController dt = new DataController();
            return dt.GetUsdValue();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
