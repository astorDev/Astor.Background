using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Newtonsoft;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Astor.Background.Management.Scraper.Tests.Playground
{
    [TestClass]
    public class Swashbuckle
    {
        [TestMethod]
        public void GeneratingSchema()
        {
            var generatorOptions = new SchemaGeneratorOptions();
            var behavior = new NewtonsoftDataContractResolver(generatorOptions, new JsonSerializerSettings());
            var repo = new SchemaRepository();

            var generator = new SchemaGenerator(generatorOptions, behavior);
            generator.GenerateSchema(typeof(Book), repo);

            var schemas = new Dictionary<string, JObject>();

            foreach (var (key, value) in repo.Schemas)
            {
                using TextWriter tw = new StringWriter();
                var oaw = new OpenApiJsonWriter(tw);
                value.SerializeAsV3(oaw);
                var s = tw.ToString();
                var jo = JObject.Parse(s);

                schemas.Add(key, jo);
            }

            Console.WriteLine(JsonConvert.SerializeObject(schemas, Formatting.Indented));
        }


        public class Book
        {
            public string Title { get; set; }

            public int Year { get; set; }

            public Author Author { get; set; }
        }

        public class Author
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}