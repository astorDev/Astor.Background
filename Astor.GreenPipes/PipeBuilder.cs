using System;
using System.Collections.Generic;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.GreenPipes
{
    public class PipeBuilder<TContext> where TContext : class, PipeContext
    {
        public IServiceCollection ServiceCollection { get; }

        private readonly List<Type> filterTypes = new();

        public PipeBuilder(IServiceCollection serviceCollection)
        {
            this.ServiceCollection = serviceCollection;
        }
        
        public PipeBuilder<TContext> Use<TFilter>() where TFilter : class, IFilter<TContext>
        {
            this.ServiceCollection.AddScoped<TFilter>();
            this.filterTypes.Add(typeof(TFilter));
            return this;
        }

        public void RegisterPipe()
        {
            this.ServiceCollection.AddScoped(sp =>
            {
                return Pipe.New<TContext>(p =>
                {
                    foreach (var filterType in this.filterTypes)
                    {
                        var filter = (IFilter<TContext>) sp.GetRequiredService(filterType);
                        p.UseFilter(filter);
                    }
                });
            });
        }
    }
}