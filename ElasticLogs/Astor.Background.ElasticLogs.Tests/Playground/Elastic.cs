using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace Astor.Background.ElasticLogs.Tests.Playground
{
    [TestClass]
    public class Elastic : Test
    {
        [TestMethod]
        public async Task InsertingDocs()
        {
            var elasticClient = this.ServiceProvider.GetRequiredService<IElasticClient>();

            var doc = new
            {
                text = "something going on",
                timestamp = DateTime.Now
            };

            var result = await elasticClient.IndexAsync(doc, i => i
                .Index("unittest-data")
            );
            
            Assert.IsTrue(result.IsValid);
        }
    }
}