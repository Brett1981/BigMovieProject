using System;
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
        public async Task<IHttpActionResult> Get([FromUri] string value)
        {
            if(value.ToLower() == "api"){
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<List<History_API>>(await History.Get.API(value),
                    new System.Net.Http.Formatting.XmlMediaTypeFormatter
                    {
                        UseXmlSerializer = true
                    })
                });
            } 
            else if(value.ToLower() == "user") {
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<List<History_User>>(await History.Get.Users(value),
                    new System.Net.Http.Formatting.XmlMediaTypeFormatter
                    {
                        UseXmlSerializer = true
                    })
                });
            }
            else { return NotFound(); }
        }

        [HttpPost,ActionName("PostGet")]
        public async Task<IHttpActionResult> PostGet([FromBody] string data)
        {
            if (data.ToLower() == "api") { return Ok(await History.Return<List<History_API>>(data)); }
            else if (data.ToLower() == "user") { return Ok(await History.Return<List<History_User>>(data)); }
            else { return NotFound(); }
        }

        [HttpPost,ActionName("Set")]
        public async Task<IHttpActionResult> Set([FromUri] string value,[FromBody] object data)
        {
            if(value.ToLower() == "api") { var s = await History.Create(value, data); if (s) { return Ok(); }else { return BadRequest(); } }
            else if(value.ToLower() == "user") { var s = await History.Create(value, data); if (s) { return Ok(); }else { return BadRequest(); } }
            else { return BadRequest(); }
        }
    }
}
