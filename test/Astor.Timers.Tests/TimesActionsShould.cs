using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Astor.Timers.Tests;

[TestClass]
public class TimesActionsShould
{
    [TestMethod]
    public async Task HandleTwentyFourHoursShift()
    {
        var times = TimesAction.WithTimeZoneShift("Do_Something", new[] { new TimeSpan(19, 20, 0) }, -24);
        times.Times.Single().Should().Be(new(19, 20, 0));
    }
    
    [TestMethod]
    public async Task HandleSmallShiftAtNight()
    {
        var times = TimesAction.WithTimeZoneShift("Do_Something", new[] { new TimeSpan(0, 40, 0) }, -3);
        times.Times.Single().Should().Be(new(21, 40, 0));
    }
    
    [TestMethod]
    public async Task HandleSameDayShift()
    {
        var times = TimesAction.WithTimeZoneShift("Do_Something", new[] { new TimeSpan(19, 20, 0) }, -2);
        times.Times.Single().Should().Be(new(17, 20, 0));
    }
}