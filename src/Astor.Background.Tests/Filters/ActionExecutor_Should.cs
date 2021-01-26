using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Filters;
using Example.Service;
using Example.Service.Controllers;
using Example.Service.Domain;
using Example.Service.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Astor.Background.Tests.Filters
{
    [TestClass]
    public class ActionExecutor_Should : FilterTest
    {
        [TestMethod]
        public async Task WriteControllerOutputToContext()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Phrases:Beginning", "Guten tag"),
            }).Build();

            this.ServiceCollection.Configure<GreetingPhrases>(configuration.GetSection("Phrases"));
            this.ServiceCollection.AddBackground(typeof(GreetingsController));
            
            var context = new EventContext(GreetingAction, new Input
            {
                BodyObject = new GreetingCandidate
                {
                    Name = "George",
                    City = new City
                    {
                        Title = "London"
                    }
                }
            });

            await this.RunPipeAsync(pipe =>
                    pipe.UseActionExecutor(),
                context
            );
            
            Assert.AreEqual("Guten tag, George from London", context.ActionResult.Output);
            Console.WriteLine(JsonConvert.SerializeObject(context.Input));
            Console.WriteLine(JsonConvert.SerializeObject(context.ActionResult));
        }
    }
}