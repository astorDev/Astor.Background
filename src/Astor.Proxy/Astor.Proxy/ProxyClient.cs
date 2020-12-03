using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Astor.Proxy
{
    public class ProxyClient
    {
        public const int StreamCopyBufferSize = 81920;
        public HttpClient HttpClient { get; }

        public ProxyClient(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }

        public async Task ForwardAsync(HttpContext context, string route)
        {
            var request = CreateProxyHttpRequest(context, route);
            var response = await this.HttpClient.SendAsync(request);
            await CopyHttpResponseAsync(context, response);
        }
        
        public static HttpRequestMessage CreateProxyHttpRequest(HttpContext context, string route)
        {
            var request = context.Request;

            var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), route);
            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                requestMessage.Content = streamContent;
            }
            
            foreach (var header in request.Headers.Where(k => k.Key != "Host"))
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            return requestMessage;
        }

        public static async Task CopyHttpResponseAsync(HttpContext context, HttpResponseMessage responseMessage)
        {
            var response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            await responseStream.CopyToAsync(response.Body, StreamCopyBufferSize, context.RequestAborted);
        }
        
    }
}