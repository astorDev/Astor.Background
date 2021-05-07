using System.Collections.Generic;
using Astor.Background.Descriptions.Core.Protocol;
using Astor.Background.Management.Protocol;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Astor.Background.Descriptions.OpenApiDocuments
{
    public static class OpenApiConversionExtensions
    {
        public static OpenApiComponents ToOpenApiComponents(this ServiceDescription description)
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

        public static OpenApiPaths ToOpenApiPaths(this Dictionary<string, HandlerDescription> descriptions)
        {
            var paths = new OpenApiPaths();
            foreach (var (key, value) in descriptions)
            {
                paths.Add($"/{key}", new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Post] = new()
                        {
                            RequestBody = new OpenApiRequestBody
                            {
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    { "application/json", new OpenApiMediaType
                                    {
                                        Schema = value.Input?.ToReferenceSchema()
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

        public static OpenApiSchema ToReferenceSchema(this InputDescription inputDescription)
        {
            if (inputDescription.IsArray)
            {
                return new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = inputDescription.ReferenceId
                        }
                    }
                };
            }
            
            return new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.Schema,
                    Id = inputDescription.ReferenceId
                }
            };
        }
        
        public static OpenApiDocument ToOpenApiDocument(this ServiceDescription description)
        {
            return new()
            {
                Info = description.Info,
                Paths = description.Handlers.ToOpenApiPaths(),
                Components = description.ToOpenApiComponents()
            };
        }
    }
}