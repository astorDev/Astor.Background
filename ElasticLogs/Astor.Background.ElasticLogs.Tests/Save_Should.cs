using System;
using System.Threading.Tasks;
using Astor.Background.Management.Protocol;
using Astor.JsonHttp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.ElasticLogs.Tests
{
    [TestClass]
    public class Save_Should
    {
        [TestMethod]
        public async Task InsertLogRecord()
        {
            var client = new WebApplicationFactory().CreateClient();

            var response = await client.PostJsonAsync("ElasticLogs_Save", new ActionResultCandidate
            {
                ActionId = "Dpd_SendReport",
                IsSuccessful = true,
                AttemptIndex = 2,
                StartTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(30)),
                EndTime = DateTime.Now,
                SourceExchange = "my-exchange",
                Event = new
                {
                    Id = 66,
                    Name = "Somebody"
                },
                Result = new []
                {
                    1, 2, 3
                }
            });

            response.EnsureSuccessStatusCode();
        }
    }
}