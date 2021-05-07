using System;
using Astor.Background.Core;
using Astor.Background.Descriptions;
using Example.Service.Controllers;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Astor.Background.Tests.Descriptions
{
    [TestClass]
    public class ServiceDescriptionGenerator_Should
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        
        [TestMethod]
        public void BuildSensibleAndSerializableDescription()
        {
            var service = Service.Parse(typeof(GreetingsController));

            var description = ServiceDescriptionBuilder.Generate(service, new OpenApiInfo
            {
                Title = "Auto tests info"
            });
            
            Assert.IsNotNull(description);
            var json = JsonConvert.SerializeObject(description, JsonSerializerSettings);
            
            Console.WriteLine(json);

            var deserializedDescription = JsonConvert.DeserializeObject(json);
            var jsonAfterDeserialization = JsonConvert.SerializeObject(deserializedDescription, JsonSerializerSettings);
            
            Console.WriteLine(jsonAfterDeserialization);
        }
    }
}