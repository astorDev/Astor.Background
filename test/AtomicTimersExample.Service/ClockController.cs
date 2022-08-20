using Astor.Background.Core.Abstractions;

public class ClockController
{
    [RunsEvery("0:00:01")]
    public async Task<string> Tick() => "sec passed";

    [RunsEvery("0:00:5")]
    public async Task<object> ActivateBird() => new { Sound = Cuckoo.MakeSound(), From = nameof(Cuckoo) };
    
    static class Cuckoo
    {
        public static string MakeSound()
        {
            if (DateTime.Now.Millisecond % 5 == 0) throw new InvalidOperationException("Can not work at the moment");
            return "Cuckoo-Cuckoo";
        }
    }
}

