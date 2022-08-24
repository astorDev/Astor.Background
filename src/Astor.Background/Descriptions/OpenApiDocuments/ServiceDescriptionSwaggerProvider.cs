using Astor.Background.Descriptions.Core.Protocol;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.Swagger;

namespace Astor.Background.Descriptions.OpenApiDocuments;

public class SingleServiceDescriptionSwaggerProvider : ISwaggerProvider
{
    readonly ServiceDescription serviceDescription;
    public SingleServiceDescriptionSwaggerProvider(ServiceDescription serviceDescription) {
        this.serviceDescription = serviceDescription;
    }
    
    public OpenApiDocument GetSwagger(string documentName, string host = null, string basePath = null) {
        return this.serviceDescription.ToOpenApiDocument();
    }
}