using System;
using System.Threading.Tasks;
using Astor.Background.Abstractions;
using Example.Service.Models;

namespace Example.Service.Controllers
{
    public class GreetingsController
    {
        [SubscribedOn("newcomer-appeared")]
        public async Task SayHelloAsync(GreetingCandidate candidate)
        {
            throw new NotImplementedException();
        }
    }
}