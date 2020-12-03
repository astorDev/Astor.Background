using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Astor.Proxy.ExampleApi.Controllers
{
    [Route("github")]
    public class GithubController : Controller
    {
        public GithubProxyClient Proxy { get; }

        public GithubController(GithubProxyClient proxy)
        {
            this.Proxy = proxy;
        }
        
        [HttpGet]
        [Authorize]
        public async Task Get()
        {
            await this.Proxy.ForwardAsync(this.HttpContext, "");
        }
    }
}