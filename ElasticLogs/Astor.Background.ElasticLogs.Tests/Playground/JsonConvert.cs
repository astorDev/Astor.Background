using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Astor.Background.ElasticLogs.Tests.Playground
{
    [TestClass]
    public class JsonConversion
    {
        [TestMethod]
        public void StringSerialization()
        {
            var exampleJsonString = "{ \"name\" : \"Alex\"}";
            Console.WriteLine(exampleJsonString);
            Console.WriteLine(JsonConvert.SerializeObject(exampleJsonString));
        }
    }
}