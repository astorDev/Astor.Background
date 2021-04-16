using System;

namespace Example.Service.Models
{
    public class GreetingCandidate
    {
        public string Name { get; set; }
        
        public City City { get; set; }
    }

    public class City
    {
        public string Title { get; set; }
        
        public int Timezone { get; set; }
        
        public DateTime CurrentTime { get; set; }
    }
}