using System;
using Astor.Background.Descriptions;
using Example.Service.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Astor.Background.Tests.Descriptions
{
    [TestClass]
    public class ServiceSchemasCollection_Should
    {
        [TestMethod]
        public void HasValidSchemaGenerator()
        {
            var generator = ServiceSchemasCollection.SchemaGenerator;
            
            Assert.IsNotNull(generator);
        }
        
        [TestMethod]
        public void GenerateSensibleDictionary()
        {
            var collection = new ServiceSchemasCollection();
            
            collection.EnsureTypeAndSubtypesAdded(typeof(GreetingCandidate));

            var dictionary = collection.ToDictionary();
            Console.WriteLine(JsonConvert.SerializeObject(dictionary, RabbitMqTest.JsonSerializerSettings));
        }
    }
}