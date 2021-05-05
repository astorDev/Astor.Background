using Astor.Background.RabbitMq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client.Events;

namespace Astor.Background.Tests.Units
{
    [TestClass]
    public class InputHelper_Should
    {
        [TestMethod]
        public void NotThrowExceptionWhenReceivingEmptyMessage()
        {
            var input = InputHelper.Parse(new BasicDeliverEventArgs());
            
            Assert.IsNotNull(input);
        }
    }
}