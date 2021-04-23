using System.Threading.Tasks;
using Astor.Background.Management.Protocol;
using Astor.Background.RabbitMq.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Astor.Background.Management.Service.Controllers
{
    public class LogsController
    {
        public IMongoDatabase Db { get; }

        public LogsController(IMongoDatabase db)
        {
            this.Db = db;
        }
        
        [SubscribedOn(ExchangeNames.Logs, DeclareExchange = true)]
        public async Task Save(ActionResultCandidate resultCandidate)
        {
            var collection = this.Db.GetCollection<object>(resultCandidate.ActionId);

            var eventJson = JsonConvert.SerializeObject(resultCandidate);
            var bsonDocument = BsonDocument.Parse(eventJson);

            await collection.InsertOneAsync(new
            {
                IsSuccessfull = resultCandidate.IsSuccessful,
                resultCandidate.Exception,
                resultCandidate.StartTime,
                resultCandidate.EndTime,
                resultCandidate.AttemptIndex,
                resultCandidate.SourceExchange,
                Event = bsonDocument["Event"],
                ResultDocument = bsonDocument["Result"]
            });
        }
    }
}