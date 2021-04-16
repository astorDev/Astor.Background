using System;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;

namespace Astor.GreenPipes
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPipe<TContext>(this IServiceCollection serviceCollection, Action<PipeBuilder<TContext>> configuration) where TContext : class, PipeContext
        {
            var builder = new PipeBuilder<TContext>(serviceCollection);
            configuration(builder);
            builder.RegisterPipe();
        }
    }
}