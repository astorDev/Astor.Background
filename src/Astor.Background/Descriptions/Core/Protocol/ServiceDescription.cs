using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace Astor.Background.Descriptions.Core.Protocol
{
    public sealed record ServiceDescription
    {
        public OpenApiInfo Info { get; set; }
        
        public Dictionary<string, HandlerDescription> Handlers { get; set; }
        
        public Dictionary<string, JObject> Schemas { get; set; } 
    }

    public sealed record HandlerDescription
    {
        public InputDescription Input { get; set; }
    }

    public class InputDescription
    {
        public bool IsArray { get; set; }
        
        public string ReferenceId { get; set; }
    }
}