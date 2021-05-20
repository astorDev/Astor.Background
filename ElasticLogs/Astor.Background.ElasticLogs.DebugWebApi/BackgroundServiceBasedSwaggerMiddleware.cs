using System.Threading.Tasks;
using Astor.Background.Descriptions.OpenApiDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace Astor.Background.ElasticLogs.DebugWebApi
{
    public class BackgroundServiceBasedSwaggerMiddleware
     {
         public OpenApiDocument Document { get; }
         public RequestDelegate RequestDelegate { get; }

         public BackgroundServiceBasedSwaggerMiddleware(OpenApiDocument document, RequestDelegate requestDelegate)
         {
             this.Document = document;
             this.RequestDelegate = requestDelegate;
         }

         public async Task InvokeAsync(HttpContext context)
         {
             if (context.Request.Path == "/swagger/v1/swagger.json")
             {
                 context.Response.ContentType = "application/json";
                 await context.Response.WriteAsync(this.Document.ToV3Json());

                 return;
             }

             await this.RequestDelegate(context);
         }
     }
}