using System.Net.Http;

namespace Astor.Proxy.ExampleApi
{
    public class JsonPlaceholderProxyClient : ProxyClient
    {
        public JsonPlaceholderProxyClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}