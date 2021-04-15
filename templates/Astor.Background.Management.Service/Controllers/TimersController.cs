using System;
using System.Linq;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Abstractions;
using Astor.Background.Management.Protocol;
using Astor.Background.Management.Service.Timers;

namespace Astor.Background.Management.Service.Controllers
{
    public class TimersController
    {
        public SchedulesStore Store { get; }

        private readonly Timers.Timers timers;

        public TimersController(SchedulesStore store)
        {
            this.Store = store;
            this.timers = new Timers.Timers(action =>
            {
                //do nothing yet
            });
        }
        
        [SubscribedOnInternal(InternalEventNames.Started)]
        public async Task Refresh()
        {
            var schedule = await this.Store.GetAllAsync();

            foreach (var row in schedule)
            {
                if (row.Times != null)
                {
                    this.timers.EnsureValid(row.ActionId, row.EveryDayAt);
                }
                else
                {
                    var interval = row.GetInterval();
                    this.timers.EnsureValid(row.ActionId, interval.Value);
                }
            }
            
            this.timers.EnsureOnly(schedule.Select(s => s.ActionId.ToString()));
        }
    }
}