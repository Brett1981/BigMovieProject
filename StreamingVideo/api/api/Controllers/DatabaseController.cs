﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using api.Resources;
using System.Web.Http.Cors;

namespace api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "GET, POST")]
    public class DatabaseController : ApiController
    {
        [HttpGet,ActionName("Get")]
        public IHttpActionResult Get([FromUri] string value)
        {
            if(value.ToLower() == "api"){
                return Ok(History.GetAPI(value)); } 
            else if(value.ToLower() == "user") {
                return Ok(History.GetUsers(value)); }
            else { return NotFound(); }
        }

        [HttpPost,ActionName("PostGet")]
        public IHttpActionResult PostGet([FromBody] string data)
        {
            if (data.ToLower() == "api") { return Ok(History.Get<List<History_API>>(data).ToList()); }
            else if (data.ToLower() == "user") { return Ok(History.Get<List<History_User>>(data).ToList()); }
            else { return NotFound(); }
        }

        [HttpPost,ActionName("Set")]
        public async Task<IHttpActionResult> Set([FromUri] string value,[FromBody] object data)
        {
            if(value.ToLower() == "api") { var s = await History.Set(value, data); if (s) { return Ok(); }else { return BadRequest(); } }
            else if(value.ToLower() == "user") { var s = await History.Set(value, data); if (s) { return Ok(); }else { return BadRequest(); } }
            else { return BadRequest(); }
        }
    }
}
