using System;
using System.Threading.Tasks;
using Astor.Background.Abstractions;
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
    }
}