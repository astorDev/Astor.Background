using System;

namespace Astor.Background.Management.Protocol
{
    public class ActionResultCandidate
    {
        public string ActionId { get; set; }
        
        public bool IsSuccessful { get; set; }
        
        public Exception Exception { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public string SourceExchange { get; set; }
        
        public int AttemptIndex { get; set; }

        public object Event { get; set; }
        
        public object Result { get; set; }
    }
}