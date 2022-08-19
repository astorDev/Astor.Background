namespace Astor.Timers;

public record TimesAction(string ActionId, TimeSpan[] Times)
{
    public static TimesAction WithTimeZoneShift(string actionId, IEnumerable<TimeSpan> rawTimes, int shift)
    {
        var times = rawTimes.Select(t =>
        {
            return shift switch
            {
                0 => t,
                < 0 => t.Add(TimeSpan.FromHours(shift + (shift < -t.Hours ? 24 : 0))),
                _ => t.Add(TimeSpan.FromHours(shift))
            };
        });

        return new(actionId, times.ToArray());
    }
}