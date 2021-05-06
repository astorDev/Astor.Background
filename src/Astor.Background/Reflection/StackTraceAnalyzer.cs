using System;
using System.Diagnostics;
using System.Reflection;

namespace Astor.Reflection
{
    public class StackTraceAnalyzer
    {
        public static Type GetCallerType()
        {
            var frame = new StackFrame(2);
            return frame.GetMethod().ReflectedType;
        }
    }
}