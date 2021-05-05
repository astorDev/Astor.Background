using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Astor.Background.Core;
using Astor.Background.Management.Protocol;
using Astor.Background.Management.Utils;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Newtonsoft;
using Swashbuckle.AspNetCore.SwaggerGen;
using DataType = Swashbuckle.AspNetCore.SwaggerGen.DataType;

namespace Astor.Background.Management.Scraper
{
    public static class Scraper
    {
        public static readonly ISerializerDataContractResolver ContractResolver = new NewtonsoftDataContractResolver(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        
        public static readonly SchemaGeneratorOptions SchemaGeneratorOptions =  new()
        {
            SchemaIdSelector = t => t.GetOpenApiId()
        };

        public static readonly SchemaGenerator SchemaGenerator = new(SchemaGeneratorOptions, ContractResolver);
        
        public static ServiceDescription GetDescription(this Service service, OpenApiInfo info)
        {
            var repo = new SchemaRepository();

            foreach (var arg in service.InputTypes)
            {
                SchemaGenerator.GenerateSchema(arg, repo);
            }

            var handlers = new Dictionary<string, HandlerDescription>();
            foreach (var (actionId, action) in service.Actions)
            {
                handlers.TryAdd(actionId, new HandlerDescription
                {
                    Input = GetReferenceSchema(action.InputType)
                });
            }
            
            return new ServiceDescription
            {
                Info = info,
                Handlers = handlers,
                Schemas = repo.GetSchemasAsJObjects()
            };
        }

        public static OpenApiSchema GetReferenceSchema(Type type)
        {
            if (type == null)
            {
                return null;
            }
            
            if (type.GetMethod("GetEnumerator") != null) 
            {
                var arrayType = type.GenericTypeArguments.Single();

                return new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = arrayType.GetOpenApiId()
                        }
                    }
                };
            }

            return new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.Schema,
                    Id = type.GetOpenApiId()
                }
            };
        }
        
        public static OpenApiComponents GetOpenApiComponents(this ServiceDescription description)
        {
            
            var componentsOnly = new ServiceDescription
            {
                Schemas = description.Schemas
            };

            var componentsJson = JsonConvert.SerializeObject(componentsOnly, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            
            return new OpenApiStringReader().ReadFragment<OpenApiComponents>(componentsJson, OpenApiSpecVersion.OpenApi3_0, out var diagnostic);
        }

        public static OpenApiPaths GetOpenApiPaths(this Dictionary<string, HandlerDescription> descriptions)
        {
            var paths = new OpenApiPaths();
            foreach (var (key, value) in descriptions)
            {
                paths.Add($"/{key}", new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Post] = new OpenApiOperation
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    { "application/json", new OpenApiMediaType
                                    {
                                        Schema = value.Input
                                    }}
                                } 
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "OK"
                                }
                            }
                        }
                    }
                });
            }

            return paths;
        }
        
        public static OpenApiDocument ToOpenApiDocument(this ServiceDescription description)
        {
            return new OpenApiDocument
            {
                Info = description.Info,
                Paths = description.Handlers.GetOpenApiPaths(),
                Components = description.GetOpenApiComponents()
            };
        }
        
        private static string camelCase(string name)
        {
            return name.First().ToString().ToLower() + new String(name.Skip(1).ToArray());
        }
    }
    
    
}