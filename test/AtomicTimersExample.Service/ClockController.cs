using Astor.Background.Core.Abstractions;

using Microsoft.Extensions.Configuration;

public class ClockController
{
    private readonly IBird bird;

    public ClockController(IBird bird)
    {
        this.bird = bird;
    }
    
    [RunsEvery("0:00:01")]
    public async Task<string> Tick() => "sec passed";

    [RunsEvery("0:00:05")]
    public async Task<object> ActivateBird()
    {
        var sound = await this.bird.MakeSound();
        return new { Sound = sound, From = this.bird.Name };
    }
    
    public interface IBird
    {
        string Name { get; }
        Task<string> MakeSound();
    }
    
    public class Cuckoo : IBird
    {
        readonly int repeats;
        readonly string? greeting;

        public Cuckoo(IConfiguration configuration) {
            this.repeats = Int32.Parse(configuration["BirdSoundRepeats"]);
            this.greeting = configuration["BirdGreeting"];
        }
        
        public string Name => nameof(Cuckoo);

        public async Task<string> MakeSound()
        {
            await Task.Delay(100);
            if (DateTime.Now.Millisecond % 3 == 0) throw new InvalidOperationException("Can not work at the moment");

            var soundParts = Enumerable.Range(0, this.repeats).Select(_ => "Cuckoo");
            return this.greeting ?? "" + String.Join("-", soundParts);
        }
    }
}

