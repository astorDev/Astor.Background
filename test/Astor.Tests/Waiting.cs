using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astor.Tests
{
    public static class Waiting
    {
        public static async Task For(Func<bool> condition, TimeSpan timeSpan)
        {
            var cancellationToken = new CancellationTokenSource(timeSpan).Token;

            while (!condition())
            {
                await Task.Delay(10, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}