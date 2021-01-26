using System;
using System.Threading.Tasks;
using Astor.Background.Core;
using GreenPipes;

namespace Astor.Background.TelegramNotifications
{
    public class ExceptionCatcherWithTelegramNotifications : IFilter<EventContext>
    {
        public TelegramNotifier Notifier { get; }

        public ExceptionCatcherWithTelegramNotifications(TelegramNotifier notifier)
        {
            this.Notifier = notifier;
        }
        
        public async Task Send(EventContext context, IPipe<EventContext> next)
        {
            try
            {
                await next.Send(context);
            }
            catch (Exception e)
            {
                await this.Notifier.SendAsync(e, context);
            }
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}