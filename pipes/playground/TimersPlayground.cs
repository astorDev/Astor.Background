namespace Astor.Background.Pipes.Playground;

[TestClass]
public class TimersPlayground
{
    [TestMethod]
    public async Task SometimesException()
    {
        _ = new Timer(
            (t) =>
            {
                var now = DateTime.Now;
                Console.WriteLine($"tick at {DateTime.Now}");
                if (now.Second % 3 == 0)
                {
                    throw new("Ticked at the wrong time");
                }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(2));

        await Task.Delay(TimeSpan.FromSeconds(5));
    }
    
    [TestMethod]
    public async Task SometimesExceptionCatched()
    {
        _ = new Timer(
            (t) =>
            {
                try
                {
                    var now = DateTime.Now;
                    Console.WriteLine($"tick at {DateTime.Now}");
                    if (now.Second % 3 == 0)
                    {
                        throw new("Ticked at the wrong time");
                    }
                }
                catch
                {
                }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(2));

        await Task.Delay(TimeSpan.FromSeconds(5));
    }
    
    [TestMethod]
    public async Task SometimesExceptionWithSafeTimer()
    {
        _ = new Timer(
            (t) =>
            {
                try
                {
                    var now = DateTime.Now;
                    Console.WriteLine($"tick at {DateTime.Now}");
                    if (now.Second % 3 == 0)
                    {
                        throw new("Ticked at the wrong time");
                    }
                }
                catch
                {
                }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(2));

        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

class SafeTimer
{
    private List<Action<Exception>> _onException = new();
    
    public static SafeTimer RunNowAndPeriodically()
    {
        var st = new SafeTimer();
        _ = new Timer(
            (t) =>
            {
                try
                {
                    var now = DateTime.Now;
                    Console.WriteLine($"tick at {DateTime.Now}");
                    if (now.Second % 3 == 0)
                    {
                        throw new("Ticked at the wrong time");
                    }
                }
                catch (Exception ex)
                {
                    foreach (var exceptionHandler in st._onException)
                    {
                        exceptionHandler(ex);
                    }
                }
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(2));

        return st;
    }
}

