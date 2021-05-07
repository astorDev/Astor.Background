using System;
using System.Threading.Tasks;
using Astor.Background.Tests.Playground;
using GreenPipes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.GreenPipes.Tests.Playground
{
    [TestClass]
    public class GreenPipes
    {
        [TestMethod]
        public async Task Consumes()
        {
            var pipe = Pipe.New<EmptyContext>(config =>
            {
                config.UseFilter(new CustomFilter());
                config.UseFilter(new CustomFilterTwo());
            });

            await pipe.Send(new EmptyContext());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task HandlesExceptions()
        {
            var pipe = Pipe.New<EmptyContext>(config =>
            {
                config.UseFilter(new ExceptionalFilter());
                config.UseFilter(new CustomFilterTwo());
            });

            await pipe.Send(new EmptyContext());
        }
    }

    public class ExceptionalFilter : IFilter<EmptyContext>
    {
        public async Task Send(EmptyContext context, IPipe<EmptyContext> next)
        {
            Console.WriteLine("filter 1 (exceptional) goes");
            throw new Exception("here exception goes");
            await next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }

    public class CustomFilter : IFilter<EmptyContext>
    {
        public Task Send(EmptyContext context, IPipe<EmptyContext> next)
        {
            Console.WriteLine("custom filter goes");
            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }

    public class CustomFilterTwo : IFilter<EmptyContext>
    {
        public async Task Send(EmptyContext context, IPipe<EmptyContext> next)
        {
            Console.WriteLine("custom filter 2 goes");
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}