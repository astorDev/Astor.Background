using System;
using Astor.Background.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.Tests
{
    public class Test
    {
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