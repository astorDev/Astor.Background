using System;
using Astor.Background.Core.Abstractions;
using Astor.Background.Management.Protocol;

namespace Astor.Background.Management.Service.Controllers
{
    public class TimersController
    {
        [SubscribedOn("")]
        public void Refresh()
        {
            Console.WriteLine("refreshing timers");
        }
    }
}