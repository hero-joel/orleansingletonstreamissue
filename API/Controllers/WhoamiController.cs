using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Test.API.Controllers
{
    [Route("[controller]")]
    public class WhoamiController : Controller
    {

        public WhoamiController( )
        {
       
        }

        [HttpGet]
        public string Get() => Dns.GetHostName();
    }
}
