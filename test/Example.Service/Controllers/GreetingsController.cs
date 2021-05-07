using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Astor.Background.Core.Abstractions;
using Astor.Background.Tests;
using Example.Service.Domain;
using Example.Service.Models;
using Microsoft.Extensions.Options;

namespace Example.Service.Controllers
{
    public class GreetingsController
    {
        public TextStore TextStore { get; }
        public GreetingPhrases Phrases { get; }
        
        public GreetingsController(IOptions<GreetingPhrases> phrases, TextStore textStore)
        {
            this.TextStore = textStore;
            this.Phrases = phrases.Value;
        }
        
        [Astor.Background.RabbitMq.Abstractions.SubscribedOn("newcomer-appeared", DeclareExchange = true)]
        public async Task<string> SayHelloAsync(GreetingCandidate candidate)
        {
            this.TextStore.TextOne = $"{this.Phrases.Beginning}, {candidate.Name} from {candidate.City.Title}";

            return this.TextStore.TextOne;
        }

        [Astor.Background.RabbitMq.Abstractions.SubscribedOn("new-group-appeared", DeclareExchange = true)]
        public string SayHelloToGroup(IEnumerable<GreetingCandidate> candidates)
        {
            return "Hello guys";
        }

        [Astor.Background.RabbitMq.Abstractions.SubscribedOn("somebody-entered-the-room", DeclareExchange = true)]
        [Astor.Background.RabbitMq.Abstractions.SubscribedOn("bump-into-somebody-at-the-street", DeclareExchange = true)]
        public string SayHelloCauseOfMultipleReasons(GreetingCandidate candidate)
        {
            return "Oh, huy";
        }

        [Astor.Background.RabbitMq.Abstractions.SubscribedOn("nicknamed-appeared", DeclareExchange = true)]
        public string SayHelloAlternative(Example.Service.Models.Alternative.GreetingCandidate candidate)
        {
            return $"Hi, let me call you '{candidate.Nickname}'";
        }

        [RunsEvery("0:00:30")]
        public string RemindAboutYourself()
        {
            this.TextStore.TextOne = "Hey there I'm ready to say hello";
            
            return this.TextStore.TextOne;
        }

        [RunsEveryDayAt("07:00")]
        [RunsEveryDayAt("08:00")]
        public string WakeUp()
        {
            return "Good morning, it's time to wake up";
        }
    }
}