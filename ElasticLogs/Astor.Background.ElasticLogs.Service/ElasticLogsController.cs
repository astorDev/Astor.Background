using System;
using System.Threading.Tasks;
using Astor.Background.Core.Abstractions;
using Astor.Background.Management.Protocol;
using Nest;

namespace Astor.Background.ElasticLogs.Service
{
    public class ElasticLogsController
    {
        public IElasticClient ElasticClient { get; }

        public ElasticLogsController(IElasticClient elasticClient)
        {
            this.ElasticClient = elasticClient;
        }
        
        [Astor.Background.RabbitMq.Abstractions.SubscribedOn(ExchangeNames.Logs)]
        public Task SaveAsync(ActionResultCandidate candidate)
        {
            //var indexName = $"BackgroundLogs-{candidate.ActionId}";
            var indexName = $"backgroundlogs-{candidate.ActionId.ToLower()}";

            var record = new
            {
                successful = candidate.IsSuccessful, 
                @event = new
                {
                    Id = 66,
                    Name = "Somebody"
                },
                startTime = candidate.StartTime,
                endTime = candidate.EndTime,
                actionId = candidate.ActionId,
                elapsedMilliseconds = (candidate.EndTime - candidate.StartTime).TotalMilliseconds,
                result = new []
                {
                    5, 6
                },
                exception = candidate.Exception,
                attemptIndex = candidate.AttemptIndex,
                sourceExchange = candidate.SourceExchange
            };


            return this.ElasticClient.IndexAsync(record, i => i.Index(indexName));
        }
    }
}