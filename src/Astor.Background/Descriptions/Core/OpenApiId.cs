using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Astor.Background.Descriptions
{
    public class OpenApiId
    {
        private readonly string id;
        
        public OpenApiId(Type type)
        {
            this.id = camelCase(type.FullName);
        }

        public OpenApiId(string id)
        {
            this.id = id;
        }

        public static implicit operator string(OpenApiId openApiId) => openApiId.id;

        public static implicit operator OpenApiId(string str) => new(str);
        
        private static string camelCase(string name)
        {
            return name.First().ToString().ToLower() + new String(name.Skip(1).ToArray());
        }

        public override string ToString()
        {
            return this.id;
        }
    }
}