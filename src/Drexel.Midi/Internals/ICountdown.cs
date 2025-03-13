using System;
using System.Threading;

namespace Drexel.Midi.Internals;

internal interface ICountdown : IDisposable
{
    bool Signal();

    void Wait(CancellationToken cancellationToken);
}