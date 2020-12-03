using System.Reflection;
using System.Threading.Tasks;
using Astor.Background.Management.Scraper;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace Example.DebugWebApi
{
    public class CustomSwaggerMiddleware
    {
        public OpenApiDocument Document { get; }
        public RequestDelegate RequestDelegate { get; }

        public CustomSwaggerMiddleware(OpenApiDocument document, RequestDelegate requestDelegate)
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