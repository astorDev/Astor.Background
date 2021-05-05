using System.Linq;
using Astor.Background.Core;
using Example.Service.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.Tests.ServiceTests
{
    [TestClass]
    public class Parse_Should
    {
        [TestMethod]
        public void GetAllMethodsWithSubscribedOnAttributes()
        {
            var service = Service.Parse(typeof(GreetingsController));

            assertAnyActionWithId(service, "Greetings_SayHello");
        }

        [TestMethod]
        public void GetAllMethodsWithEveryAttribute()
        {
            var service = Service.Parse(typeof(GreetingsController));
            
            assertAnyActionWithId(service, "Greetings_RemindAboutYourself");
        }

        [TestMethod]
        public void GetAllMethodsWithRunsEveryDayAtAttribute()
        {
            var service = Service.Parse(typeof(GreetingsController));
            
            assertAnyActionWithId(service, "Greetings_WakeUp");
        }

        private static void assertAnyActionWithId(Service service, string actionId)
        {
            Assert.IsTrue(service.Actions.Any(s => s.Key == actionId));
        }
        
    }
}