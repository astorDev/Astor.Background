using System;
using Astor.Background.Core.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.Tests.CoreAttributes
{
    [TestClass]
    public class RunsEvery_Should
    {
        [TestMethod]
        public void ParseIntervals()
        {
            var attribute = new RunsEveryAttribute("0:00:10");
            
            Assert.AreEqual(TimeSpan.FromSeconds(10), attribute.Interval);
        }
    }
}