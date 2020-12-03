using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Example.DebugWebApi.Controllers
{
    [Route("")]
    public class DebugController : Controller
    {
        public Astor.Background.Service Service { get; }
        public IServiceProvider ServiceProvider { get; }

        public DebugController(Astor.Background.Service service, IServiceProvider serviceProvider)
        {
            this.Service = service;
            this.ServiceProvider = serviceProvider;
        }
        
        [HttpPost("{actionId}")]
        public Task<object> HandleAsync(string actionId, [FromBody] object request)
        {
            var inputJson = JsonConvert.SerializeObject(request);
            return this.Service.RunAsync(actionId, inputJson, this.ServiceProvider);
        }
    }
}