using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Astor.Proxy.ExampleApi.Controllers
{
    [Route("posts")]
    public class PostsController : Controller
    {
        public JsonPlaceholderProxyClient Proxy { get; }

        public PostsController(JsonPlaceholderProxyClient proxy)
        {
            this.Proxy = proxy;
        }
        
        [HttpPost]
        public Task CreateAsync()
        {
            return this.Proxy.ForwardAsync(this.HttpContext, "posts");
        } 
    }
}