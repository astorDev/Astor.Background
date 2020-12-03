using System.Net.Http;

namespace Astor.Proxy.ExampleApi
{
    public class GithubProxyClient : ProxyClient
    {
        public GithubProxyClient(HttpClient httpClient) : base(httpClient)
        {
        }
    }
}