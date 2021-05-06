using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Reflection.Tests
{
    [TestClass]
    public class StackTraceAnalyzer_Should
    {
        [TestMethod]
        public void ReturnUpperCallingMethod()
        {
            var typeName = Called.GetCallerName();
            
            Assert.AreEqual(nameof(StackTraceAnalyzer_Should), typeName);
        }
    }
    
    public static class Called
    {
        public static string GetCallerName()
        {
            var method = StackTraceAnalyzer.GetCallerType();
            return method.Name;
        }
    }
}