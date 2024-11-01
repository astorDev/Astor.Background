public class SafeTimer(Timer innerTimer)
{
    public void Stop() {
        innerTimer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    public void Start(TimeSpan interval) {
        innerTimer.Change(TimeSpan.Zero, interval);
    }

    public static SafeTimer Unstarted(Action action, Action<Exception>? onException = null) {
        var innerTimer = new Timer(
            (t) => {
                try {
                    action();
                }
                catch (Exception ex) {
                    onException?.Invoke(ex);
                }
            },
            null,
            Timeout.Infinite,
            Timeout.Infinite
        );

        return new SafeTimer(innerTimer);
    }

    public static SafeTimer RunNowAndPeriodically(
        TimeSpan interval, 
        Action action,
        Action<Exception>? onException = null
    )
    {
        var innerTimer = new Timer(
            (t) => {
                try {
                    action();
                }
                catch (Exception ex) {
                    onException?.Invoke(ex);
                }
            },
            null,
            TimeSpan.Zero,
            interval
        );

        return new SafeTimer(innerTimer);
    }
}