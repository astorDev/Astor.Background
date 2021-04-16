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
        
        public async Task<ActionSchedule> AddAsync(ActionSchedule schedule)
        {
            await this.SchedulesCollection.InsertOneAsync(schedule);
            return await this.SchedulesCollection.Find(s => s.ActionId == schedule.ActionId).SingleAsync();
        }

        public async Task<IEnumerable<ActionSchedule>> GetAllAsync()
        {
            return await this.SchedulesCollection.Find(_ => true).ToListAsync();
        }
    }
}