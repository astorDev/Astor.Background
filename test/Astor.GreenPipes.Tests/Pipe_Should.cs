using System;
using System.Threading.Tasks;
using Astor.Background.Tests;
using Astor.Background.Tests.Playground;
using Astor.Tests;
using GreenPipes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.GreenPipes.Tests
{
    [TestClass]
    public class Pipe_Should : Test
    {
        [TestMethod]
        public async Task CallFilterProvidedByDependencyInjection()
        {
            await this.runAsync(p =>
            {
                p.Use<FilterOne>();
            }, textStore =>
            {
                Assert.AreEqual("done", textStore.TextOne);
                Assert.IsNull(textStore.TextTwo);
                Assert.IsNull(textStore.TextThree);
            });
        }

        [TestMethod]
        public async Task CallThreeChainedMiddlewares()
        {
            await this.runAsync(p =>
            {
                p.Use<FilterOne>();
                p.Use<FilterTwo>();
                p.Use<FilterThree>();
            }, textStore =>
            {
                Assert.AreEqual("done", textStore.TextOne);
                Assert.AreEqual("done", textStore.TextTwo);
                Assert.AreEqual("done", textStore.TextThree);
            });
        }

        [TestMethod]
        public async Task CallMiddlewareOnlyWhileTheyPassExecution()
        {
            await this.runAsync(p =>
            {
                p.Use<FilterOne>();
                p.Use<FilterTwoBreaking>();
                p.Use<FilterThree>();
            }, textStore =>
            {
                Assert.AreEqual("done", textStore.TextOne);
                Assert.AreEqual("done", textStore.TextTwo);
                Assert.IsNull(textStore.TextThree);
            });
        }

        private async Task runAsync(Action<PipeBuilder<EmptyContext>> pipeArrangement, Action<TextStore> assertion)
        {
            var pipeBuilder = new PipeBuilder<EmptyContext>(this.ServiceCollection);
            pipeArrangement(pipeBuilder);
            pipeBuilder.RegisterPipe();

            var pipe = this.ServiceProvider.GetRequiredService<IPipe<EmptyContext>>();
            await pipe.Send(new EmptyContext());
            
            var textStore = this.ServiceProvider.GetRequiredService<TextStore>();

            assertion(textStore);
        }
        
        
    public class FilterOne : IFilter<EmptyContext>
    {
        public TextStore TextStore { get; }

        public FilterOne(TextStore textStore)
        {
            this.TextStore = textStore;
        }
        
        public Task Send(EmptyContext context, IPipe<EmptyContext> next)
        {
            this.TextStore.TextOne = "done";
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }

    public class FilterTwo : IFilter<EmptyContext>
    {
        public TextStore TextStore { get; }

        public FilterTwo(TextStore textStore)
        {
            this.TextStore = textStore;
        }
        
        public Task Send(EmptyContext context, IPipe<EmptyContext> next)
        {
            this.TextStore.TextTwo = "done";
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
    
    public class FilterTwoBreaking : IFilter<EmptyContext>
    {
        public TextStore TextStore { get; }

        public FilterTwoBreaking(TextStore textStore)
        {
            this.TextStore = textStore;
        }
        
        public async Task Send(EmptyContext context, IPipe<EmptyContext> next)
        {
            this.TextStore.TextTwo = "done";
        }

        public void Probe(ProbeContext context)
        {
        }
    }

    public class FilterThree : IFilter<EmptyContext>
    {
        public TextStore TextStore { get; }

        public FilterThree(TextStore textStore)
        {
            this.TextStore = textStore;
        }
        
        public Task Send(EmptyContext context, IPipe<EmptyContext> next)
        {
            this.TextStore.TextThree = "done";
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
    }

}