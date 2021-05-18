using System;
using Astor.Background.Core;
using Astor.Background.Descriptions;
using Astor.Background.Descriptions.OpenApiDocuments;
using Example.Service.Controllers;
using Microsoft.OpenApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Astor.Background.Tests.Descriptions
{
    [TestClass]
    public class OpenApiConversionExtensions_Should
    {
        [TestMethod]
        public void GenerateAdequateDocument()
        {
            var service = Service.Parse(typeof(GreetingsController));
            var description = ServiceDescriptionBuilder.Generate(service, new OpenApiInfo
            {
                Title = "xx",
                Version = "1.0"
            });

            Console.WriteLine(JsonConvert.SerializeObject(description, Test.JsonSerializerSettings));
            var openApiDoc = description.ToOpenApiDocument();
            Console.WriteLine(openApiDoc.ToV3Json());
        }
    }
}