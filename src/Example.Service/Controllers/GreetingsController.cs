using System.Collections.Generic;
using System.Threading.Tasks;
using Astor.Background.Core.Abstractions;
using Example.Service.Domain;
using Example.Service.Models;
using Microsoft.Extensions.Options;

namespace Example.Service.Controllers
{
    public class GreetingsController
    {
        public GreetingPhrases Phrases { get; }
        
        public GreetingsController(IOptions<GreetingPhrases> phrases)
        {
            this.Phrases = phrases.Value;
        }
        
        [SubscribedOn("newcomer-appeared")]
        public async Task<string> SayHelloAsync(GreetingCandidate candidate)
        {
            return $"{this.Phrases.Beginning}, {candidate.Name} from {candidate.City.Title}";
        }

        [SubscribedOn("new-group-appeared")]
        public string SayHelloToGroup(IEnumerable<GreetingCandidate> candidates)
        {
            return "Hello guys";
        }

        [SubscribedOn("somebody-entered-the-room")]
        [SubscribedOn("bump-into-somebody-at-the-street")]
        public string SayHelloCauseOfMultipleReasons(GreetingCandidate candidate)
        {
            return "Oh, huy";
        }

        [SubscribedOn("nicknamed-appeared")]
        public string SayHelloAlternative(Example.Service.Models.Alternative.GreetingCandidate candidate)
        {
            return $"Hi, let me call you '{candidate.Nickname}'";
        }
    }
}