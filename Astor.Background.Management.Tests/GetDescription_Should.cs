using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Astor.Background.Core;
using Astor.Background.Management.Protocol;
using Example.Service.Models;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Astor.Background.Management.Scraper.Tests
{
    [TestClass]
    public class GetDescription_Should
    {
        [TestMethod]
        public void ProvideAdequateDescription()
        {
            var service = Service.Parse(typeof(Chat).Assembly);
            var description = service.GetDescription(new OpenApiInfo
            {
                Title = "Example",
                Version = "1.0"
            });
            
            Console.WriteLine(JsonConvert.SerializeObject(description, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            }));
            
            Assert.AreEqual("Example", description.Info.Title);
            Assert.AreEqual("1.0", description.Info.Version);
            
            Assert.AreEqual(1, description.Handlers.Count);
            Assert.AreEqual("Greetings_SayHello", description.Handlers.Keys.Single());
        }

        [TestMethod]
        public void ExportToOpenApiDocument()
        {
            var service = Service.Parse(typeof(Chat).Assembly);
            var description = service.GetDescription(new OpenApiInfo
            {
                Title = "Example",
                Version = "1.0",
                Extensions = null
            });

            var document = description.ToOpenApiDocument();

            using TextWriter tw = new StringWriter();
            var oaw = new OpenApiJsonWriter(tw);
            document.SerializeAsV3(oaw);
            var s = tw.ToString();
            Console.WriteLine(s);
        }
    }
}