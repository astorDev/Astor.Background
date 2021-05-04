using System;
using System.Threading;
using System.Threading.Tasks;
using Astor.Background.Management.Protocol;
using Astor.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Astor.Background.Management.Service.Tests
{
    [TestClass]
    public class Logs_Should
    {
        private readonly string actionId = Guid.NewGuid().ToString();
        
        [TestMethod]
        [Ignore("threads problem, fails for now - requires further investigation")]
        public async Task BeWrittenToDb()
        {
            var host = Test.StartHost();

            var rabbitChannel = host.Services.GetRequiredService<IModel>();
            var mongoDb = host.Services.GetRequiredService<IMongoDatabase>();
            
            rabbitChannel.PublishJson(ExchangeNames.Logs, new ActionResultCandidate
            {
                ActionId = this.actionId
            });

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            object document = null;
            
            while (document == null)
            {
                var collection = mongoDb.GetCollection<object>(this.actionId);
                document = await collection.Find(x => true).SingleOrDefaultAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            
            Console.WriteLine(JsonConvert.SerializeObject(document));
        }
    }
}