using System;
using System.Reflection;

namespace Astor.Background.Core
{
    public class ActionId
    {
        public string Id { get; }
        
        public string Receiver { get; }
        
        public string Method { get; }

        public ActionId(string receiver, string method)
        {
            this.Receiver = receiver;
            this.Method = method;
            this.Id = $"{this.Receiver}_{this.Method}";
        }
        
        public static implicit operator string(ActionId actionId) => actionId.Id; 

        public override string ToString()
        {
            return this.Id;
        }
    }
}