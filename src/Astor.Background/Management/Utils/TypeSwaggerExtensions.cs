using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Astor.Background.Management.Utils
{
    public static class OpenApiHelper
    {
        public static string GetOpenApiId(this Type type)
        {
            //Note : camelCase is required and used internally in swagger for unknown reason
            
            return camelCase(type.FullName);
        }

        public static Dictionary<string, JObject> GetSchemasAsJObjects(this SchemaRepository repo)
        {
            var schemas = new Dictionary<string, JObject>();

            foreach (var (key, value) in repo.Schemas)
            {
                using TextWriter tw = new StringWriter();
                var oaw = new OpenApiJsonWriter(tw);
                value.SerializeAsV3(oaw);
                var s = tw.ToString();
                var jo = JObject.Parse(s);
                
                schemas.Add(key, jo);
            }

            return schemas;
        }
        
        private static string camelCase(string name)
        {
            return name.First().ToString().ToLower() + new String(name.Skip(1).ToArray());
        }
    }
}