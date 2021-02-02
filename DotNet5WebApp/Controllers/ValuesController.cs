using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet5WebApp.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [MapToApiVersion("1.0")]
        public IEnumerable<string> GetV1()
        {
            return Enumerable.Range(1, 2).Select(index => index.ToString())
            .ToArray();
        }


        [HttpGet]
        [MapToApiVersion("2.0")]
        public IEnumerable<string> GetV2()
        {
            return Enumerable.Range(1, 5).Select(index => index.ToString())
            .ToArray();
        }

    }
}
