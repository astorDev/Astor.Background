using System.Linq;
using System.Threading.Tasks;
using Astor.Background.Filters;
using Example.Service.Controllers;
using Example.Service.Models;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.Tests.Filters
{
    [TestClass]
    public class JsonBodyDeserialization_Should : FilterTest
    {
        [TestMethod]
        public async Task WriteInputBody()
        {
            var context = new EventContext(GreetingAction, new Input
            {
                BodyString = "{ 'name' : 'Clark'}"
            });

            await this.RunPipeAsync(p => 
                    p.UseConsoleInputWriter()
                        .UseJsonBodyDeserializer(), 
                context);

            var candidate = (GreetingCandidate) context.Input.BodyObject;
            
            Assert.AreEqual("Clark",candidate.Name);
        }
    }
}