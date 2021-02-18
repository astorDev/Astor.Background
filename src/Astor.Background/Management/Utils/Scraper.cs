using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Astor.Background.Core;
using Astor.Background.Management.Protocol;
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
        public static ServiceDescription GetDescription(this Service service, OpenApiInfo info)
        {
            var handlerArgs = service.Subscriptions.Select(s => s.Action.InputType);
            var repo = new SchemaRepository();
            var generatorOptions = new SchemaGeneratorOptions
            {
                SchemaIdSelector = (t) => camelCase(t.Name)
            };
            var behavior = new NewtonsoftDataContractResolver(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var generator = new SchemaGenerator(generatorOptions, behavior);

            foreach (var arg in handlerArgs)
            {
                generator.GenerateSchema(arg, repo);
            }
            
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
            
            return new ServiceDescription
            {
                Info = info,
                Handlers = service.Subscriptions.ToDictionary(
                    s => s.Action.Id, 
                    s => new HandlerDescription
                    {
                        Input = GetSchema(s.Action.InputType)
                    }),
                Schemas = schemas
            };
        }

        public static OpenApiSchema GetSchema(Type type)
        {
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
                            Id = camelCase(arrayType.Name)
                        }
                    }
                };
            }

            return new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.Schema,
                    Id = camelCase(type.Name)
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