using System.Threading;

namespace Drexel.Midi.Internals;

internal sealed class UnlimitedCountdown : ICountdown
{
    public UnlimitedCountdown()
    {
    }

    public bool Signal() => false;

    public void Wait(CancellationToken cancellationToken)
    {
        using ManualResetEventSlim waiting = new();
        cancellationToken.Register(() => waiting.Set());
        waiting.Wait();

        cancellationToken.ThrowIfCancellationRequested();
    }

    public void Dispose()
    {
        // Nothing to dispose.
    }
}