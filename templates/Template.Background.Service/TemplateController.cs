using System;
using System.Threading.Tasks;
using Astor.Background.Core.Abstractions;

namespace Template.Background.Service
{
    public class TemplateController
    {
        [Astor.Background.RabbitMq.Abstractions.SubscribedOn("numbers.supply", DeclareExchange = true)]
        public Task<int> TryParse(string num)
        {
            Int32.TryParse(num, out var n);
            return Task.FromResult(n);
        }
        
        [RunsEvery("00:00:10")]
        public Task<string> GetTime()
        {
            return Task.FromResult($"Current Time is {DateTime.Now}");
        }
    }
}