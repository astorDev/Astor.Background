using System;
using System.Collections.Generic;
using System.Linq;
using Astor.Background.Core;
using Astor.Background.Descriptions.Core.Protocol;
using Microsoft.OpenApi.Models;
using Namotion.Reflection;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.Generation;
using Action = Astor.Background.Core.Action;

namespace Astor.Background.Descriptions
{
    public class ServiceDescriptionBuilder
    {
        private readonly ServiceSchemasCollection serviceSchemasCollection = new();
        
        public static readonly JsonSchemaGenerator SchemaGenerator = new(new JsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3,
        });
        
        private readonly Dictionary<string, HandlerDescription> handlers = new();

        public void Use(Action action)
        {
            if (action.InputType == null)
            {
                this.handlers.TryAdd(action.Id, new HandlerDescription());
                return;
            }

            var enumerableGenericArg = action.InputType.GetEnumerableItemType();
            if (enumerableGenericArg != null)
            {
                this.UseActionWithInput(action, enumerableGenericArg, true);
                return;
            }
            
            this.UseActionWithInput(action, action.InputType, false);
        }

        private void UseActionWithInput(Action action, Type referenceType, bool isArray)
        {
            this.handlers.TryAdd(action.Id, new HandlerDescription
            {
                Input = new InputDescription
                {
                    IsArray = isArray,
                    ReferenceId = new OpenApiId(referenceType)
                }
            });
            this.serviceSchemasCollection.EnsureTypeAndSubtypesAdded(referenceType);
        }


        public ServiceDescription Build(OpenApiInfo info)
        {
            return new()
            {
                Info = info,
                Handlers = this.handlers,
                Schemas = this.serviceSchemasCollection.ToDictionary()
            };
        }
        
        public static ServiceDescription Generate(Service service, OpenApiInfo info)
        {
            var builder = new ServiceDescriptionBuilder();
            foreach (var action in service.Actions.Values)
            {
                builder.Use(action);
            }

            return builder.Build(info);
        }

        public static JObject SchemaJObject(Type type)
        {
            var schema = SchemaGenerator.Generate(type);

            var json = schema.ToJson();
            return JObject.Parse(json);
        }
        
        public static InputDescription GetInputDescription(Type type)
        {
            if (type == null)
            {
                return null;
            }
            
            if (type.GetMethod("GetEnumerator") != null) 
            {
                var arrayType = type.GenericTypeArguments.Single();

                return new InputDescription
                {
                    IsArray = true,
                    ReferenceId = new OpenApiId(arrayType)
                };
            }

            return new InputDescription()
            {
                ReferenceId = new OpenApiId(type),
            };
        }
    }
}