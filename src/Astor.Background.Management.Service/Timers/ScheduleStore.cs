using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Astor.Background.Management.Service.Timers
{
    public class SchedulesStore
    {
        public IMongoCollection<ActionSchedule> SchedulesCollection { get; }

        public SchedulesStore(IMongoCollection<ActionSchedule> schedulesCollection)
        {
            this.SchedulesCollection = schedulesCollection;
        }

        public async Task<ActionSchedule> AddOrUpdateAsync(ActionSchedule schedule)
        {
            if (await this.SchedulesCollection.Find(s => s.ActionId == schedule.ActionId)
                .AnyAsync())
            {
                await this.SchedulesCollection.ReplaceOneAsync(s => s.ActionId == schedule.ActionId, schedule);
            }
            else
            {
                await this.SchedulesCollection.InsertOneAsync(schedule);
            }

            return await this.SchedulesCollection.Find(s => s.ActionId == schedule.ActionId).SingleAsync();
        }

        public async Task<IEnumerable<ActionSchedule>> GetAllAsync()
        {
            return await this.SchedulesCollection.Find(_ => true).ToListAsync();
        }
    }
}