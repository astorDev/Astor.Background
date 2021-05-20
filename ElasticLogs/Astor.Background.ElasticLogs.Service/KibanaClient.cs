using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Astor.Background.ElasticLogs.Service
{
    public class KibanaClient
    {
        public HttpClient Client { get; }
        public ILogger<KibanaClient> Logger { get; }

        public KibanaClient(HttpClient client, ILogger<KibanaClient> logger)
        {
            this.Client = client;
            this.Logger = logger;
        }

        public async Task ImportDashboardAsync(string json)
        {
            const string uri = "/api/kibana/dashboards/import?force=true";
            
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var stringContent = new StringContent(json);
            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            stringContent.Headers.Add("kbn-xsrf", "reporting");
            request.Content = stringContent;

            var response = await this.Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                this.Logger.LogError(await response.Content?.ReadAsStringAsync());
                response.EnsureSuccessStatusCode();
            }
        }
    }
}