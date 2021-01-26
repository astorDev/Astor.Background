using System.Collections.Generic;

namespace Astor.Background.Core
{
    public class Input
    {
        public Dictionary<string, object> Headers { get; set; }
        
        public string BodyString { get; set; }
        
        public object BodyObject { get; set; }
    }
}