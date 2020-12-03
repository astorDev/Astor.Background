using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Astor.Proxy.ExampleApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Proxy.Tests
{
    [TestClass]
    public class Github_Should
    {
        [TestMethod]
        public async Task ReturnUnauthorized_WhenRunWithoutToken()
        {
            var waf = new WebApplicationFactory<Startup>();
            var client = waf.CreateClient();

            var response = await client.GetAsync("github");
            
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        
        [TestMethod]
        public async Task CopyGithubResponse_WhenValidTokenPassed()
        {
            var waf = new WebApplicationFactory<Startup>();
            var client = waf.CreateClient();
            var directClient = new HttpClient();
            var userAgentHeader = new ProductInfoHeaderValue("code", "1.0");

            var proxyRequest = new HttpRequestMessage(HttpMethod.Get, "github");
            proxyRequest.Headers.UserAgent.Add(userAgentHeader);
            var jwt = Jwt.Create();
            Console.WriteLine(jwt);
            proxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            var proxyResponse = await client.SendAsync(proxyRequest);

            var request = new HttpRequestMessage(HttpMethod.Get, "http://api.github.com");
            request.Headers.UserAgent.Add(userAgentHeader);
            var directResponse = await directClient.SendAsync(request);

            
            Assert.AreEqual(directResponse.StatusCode, proxyResponse.StatusCode);
            Assert.AreEqual(await directResponse.Content.ReadAsStringAsync(), await proxyResponse.Content.ReadAsStringAsync());
        }
    }
}