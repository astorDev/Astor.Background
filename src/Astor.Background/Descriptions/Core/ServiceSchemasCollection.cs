using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Astor.Background.Descriptions
{
    public class ServiceSchemasCollection
    {
        public static readonly JsonSchemaGenerator SchemaGenerator = new(new JsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            }
        });
        
        private readonly Dictionary<string, JObject> schemas = new();

        public void EnsureTypeAndSubtypesAdded(Type type)
        {
            var schemaId = new OpenApiId(type);
            if (this.schemas.ContainsKey(schemaId))
            {
                return;
            }
            
            var schema = SchemaGenerator.Generate(type);
            var json = schema.ToJson();
            var schemaJObject = JObject.Parse(json);
            
            this.EnsureSubtypesAddedAndReferencedCorrectly(type, schema, schemaJObject);

            schemaJObject.Remove("$schema");
            schemaJObject.Remove("additionalProperties");

            this.schemas.Add(schemaId, schemaJObject);
        }

        private void EnsureSubtypesAddedAndReferencedCorrectly(Type type, JsonSchema schema, JObject schemaJObject)
        {
            foreach (var schemaProperty in schema.Properties)
            {
                if (this.tryAddDirectlyReferencedObject(type, schemaJObject, schemaProperty)) continue;
                if (this.tryAdObjectReferencedAsEnumerable(type, schemaJObject, schemaProperty)) continue;
                if (this.tryAddEnums(type, schemaJObject, schemaProperty)) continue;
            }
        }

        private bool tryAdObjectReferencedAsEnumerable(Type type, JObject schemaJObject, KeyValuePair<string, JsonSchemaProperty> schemaProperty)
        {
            if (schemaProperty.Value.Item?.Reference != null)
            {
                var typeProperty = GetProperty(type, schemaProperty);
                var enumerableTypeArg = typeProperty!.PropertyType.GetEnumerableItemType();
                this.EnsureTypeAndSubtypesAdded(enumerableTypeArg!);

                SetArrayRefToComponent(schemaJObject, schemaProperty, enumerableTypeArg);

                return true;
            }

            return false;
        }

        private bool tryAddDirectlyReferencedObject(Type type, JObject schemaJObject, KeyValuePair<string, JsonSchemaProperty> schemaProperty)
        {
            if (schemaProperty.Value.OneOf.Any())
            {
                var propertyType = GetProperty(type, schemaProperty)!.PropertyType;

                SetPropertyToRef(propertyType, schemaJObject, schemaProperty);
                this.EnsureTypeAndSubtypesAdded(propertyType);

                return true;
            }

            return false;
        }

        private bool tryAddEnums(Type type, JObject schemaJObject, KeyValuePair<string, JsonSchemaProperty> schemaProperty)
        {
            if (schemaProperty.Value.Reference != null)
            {
                var propertyType = GetProperty(type, schemaProperty)!.PropertyType;
                
                SetPropertyToRef(propertyType, schemaJObject, schemaProperty);
                this.EnsureTypeAndSubtypesAdded(propertyType);

                return true;
            }

            return false;
        }
        

        private static PropertyInfo GetProperty(Type type, KeyValuePair<string, JsonSchemaProperty> schemaProperty)
        {
            var searchedPropertyName = PascalCaseFromCamelCase(schemaProperty.Key);
            var property = type.GetProperty(searchedPropertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Unable to find property {searchedPropertyName} in {type.FullName}");
            }

            return property;
        }

        public static string PascalCaseFromCamelCase(string source)
        {
            return $"{source[0].ToString().ToUpper()}{new string(source.Skip(1).ToArray())}";
        }
        
        public Dictionary<string, JObject> ToDictionary()
        {
            return this.schemas.ToDictionary(s => s.Key, s => s.Value);
        }
        
        private static void SetArrayRefToComponent(JObject schemaJObject, KeyValuePair<string, JsonSchemaProperty> schemaProperty, Type enumerableTypeArg)
        {
            schemaJObject["properties"]![schemaProperty.Key]!["items"]!["$ref"] = ComponentRefPath(enumerableTypeArg);
        }

        private static void SetPropertyToRef(Type propertyType, JObject schemaJObject, KeyValuePair<string, JsonSchemaProperty> schemaProperty)
        {
            var jsonForRef = $"{{ \"$ref\": \"{ComponentRefPath(propertyType)}\" }}";

            var propertiesNode = schemaJObject["properties"];
            propertiesNode![schemaProperty.Key] = JObject.Parse(jsonForRef);
        }

        private static string ComponentRefPath(Type type)
        {
            return $"#/components/schemas/{new OpenApiId(type)}";
        }
    }
}