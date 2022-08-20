using System;

namespace Astor.Background.Core
{
    public class ActionResult
    {
        public object? Output { get; set; }
        
        public Exception? Exception { get; set; }
    }
}