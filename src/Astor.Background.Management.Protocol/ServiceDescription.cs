using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;

namespace Astor.Background.Management.Protocol
{
    public sealed record ServiceDescription
    {
        public OpenApiInfo Info { get; set; }
        
        public Dictionary<string, HandlerDescription> Handlers { get; set; }
        
        public Dictionary<string, JObject> Schemas { get; set; } 
    }

    public sealed record HandlerDescription
    {
        public string Input { get; set; }
    }
}