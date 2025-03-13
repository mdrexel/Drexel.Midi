namespace Drexel.Midi.Internals;

internal static class Countdown
{
    public static ICountdown Create(int limit) => limit < 0
        ? new UnlimitedCountdown()
        : new BoundedCountdown(limit);
}