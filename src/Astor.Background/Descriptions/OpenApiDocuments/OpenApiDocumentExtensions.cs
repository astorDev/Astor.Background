using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace Astor.Background.Descriptions.OpenApiDocuments
{
    public static class OpenApiDocumentExtensions
    {
        public static string ToV3Json(this OpenApiDocument document)
        {
            using var tw = new StringWriter();
            var oaw = new OpenApiJsonWriter(tw);
            document.SerializeAsV3(oaw);
            return tw.ToString();
        }
    }
}