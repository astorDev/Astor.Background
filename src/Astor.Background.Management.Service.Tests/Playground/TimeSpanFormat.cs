using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Background.Management.Service.Tests.Playground
{
    [TestClass]
    public class TimeSpanFormat
    {
        [TestMethod]
        public void DisplaysHoursAndMinutes()
        {
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString("hh\\:mm"));
        }
    }
}