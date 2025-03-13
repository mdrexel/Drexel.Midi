using System.Threading;

namespace Drexel.Midi.Internals;

internal sealed class BoundedCountdown : ICountdown
{
    private readonly CountdownEvent _event;

    public BoundedCountdown(int count)
    {
        _event = new(count);
    }

    public bool Signal() =>
        _event.Signal();

    public void Wait(CancellationToken cancellationToken) =>
        _event.Wait(cancellationToken);

    public void Dispose()
    {
        _event.Dispose();
    }
}