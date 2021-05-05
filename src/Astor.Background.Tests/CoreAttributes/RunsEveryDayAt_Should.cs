using System;
using Astor.Background.Core.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.Tests.CoreAttributes
{
    [TestClass]
    public class RunsEveryDayAt_Should
    {
        [TestMethod]
        public void ParseTime()
        {
            var attribute = new RunsEveryDayAtAttribute("07:00");
            
            Assert.AreEqual(TimeSpan.FromHours(7), attribute.Time);
        }
    }
}