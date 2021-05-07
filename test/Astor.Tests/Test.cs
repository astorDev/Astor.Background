using System;
using Astor.Background.Tests;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Astor.Tests
{
    public class Test
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        
        private IServiceCollection serviceCollection;
        public IServiceCollection ServiceCollection =>
            this.serviceCollection ??= this.CreateBaseServiceCollection();

        private IServiceProvider serviceProvider;
        public IServiceProvider ServiceProvider =>
            this.serviceProvider ??= this.ServiceCollection.BuildServiceProvider();

        public virtual IServiceCollection CreateBaseServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<TextStore>();

            return serviceCollection;
        }
    }
}