using System;
using System.Collections.Generic;

namespace Example.Service.Models
{
    public class GreetingCandidate
    {
        public string Name { get; set; }
        
        public string[] Tags { get; set; }
        
        public City City { get; set; }
        
        public DayPeriod DayPeriod { get; set; }

        public Friend[] Friends { get; set; }
    }

    public class City
    {
        public string Title { get; set; }
        
        public int Timezone { get; set; }
        
        public DateTime CurrentTime { get; set; }
    }

    public class Friend
    {
        public string Name { get; set; }
    }

    public enum DayPeriod
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }
}