using Microsoft.Extensions.Hosting;

namespace Astor.Background.Management.Service.Tests
{
    public class Test
    {
        public static IHost StartHost()
        {
            var host = Program.CreateHost(new[]
            {
                "--ConnectionStrings:Rabbit=amqp://localhost:5672",
                "--ConnectionStrings:Mongo=mongodb://localhost:27017"
            });
            host.Init();
            host.RunAsync();

            return host;
        }
    }
}