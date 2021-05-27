using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astor.Tests
{
    public static class Waiting
    {
        public static async Task For(Func<bool> condition, TimeSpan timeout)
        {
            var cancellationToken = new CancellationTokenSource(timeout).Token;

            while (!condition())
            {
                await Task.Delay(10, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public static async Task For(TimeSpan timeSpan)
        {
            var cancellationToken = new CancellationTokenSource(timeSpan).Token;

            await For(cancellationToken);
        }

        public static async Task For(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10);
            }
        }
    }
}