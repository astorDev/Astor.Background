using System.Threading.Tasks;
using Astor.Background;
using Astor.Background.Core;
using GreenPipes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Example.DebugWebApi.Controllers
{
    [Route("")]
    public class DebugController : Controller
    {
        public IPipe<EventContext> Pipe { get; }
        public Astor.Background.Core.Service Service { get; }

        public DebugController(IPipe<EventContext> pipe, Astor.Background.Core.Service service)
        {
            this.Pipe = pipe;
            this.Service = service;
        }
        
        [HttpPost("{actionId}")]
        public async Task<object> HandleAsync(string actionId, [FromBody] object request)
        {
            var action = this.Service.Actions[actionId];
            var inputJson = JsonConvert.SerializeObject(request);
            var context = new EventContext(action, new Input
            {
                BodyString = inputJson
            });
            
            await this.Pipe.Send(context);
            return context.ActionResult.Output;
        }
    }
}