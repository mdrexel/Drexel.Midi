using System;
using System.Collections.Generic;
using System.Linq;

namespace Drexel.Midi.Internals;

internal sealed class CompositeDisposable : IDisposable
{
    private readonly IEnumerable<IDisposable> _disposables;

    public CompositeDisposable(IEnumerable<IDisposable> disposables)
    {
        _disposables = disposables;
    }

    public CompositeDisposable(params IEnumerable<IEnumerable<IDisposable>> disposables)
    {
        _disposables = disposables.SelectMany(x => x);
    }

    public void Dispose()
    {
        List<Exception> exceptions = [];
        foreach (IDisposable disposable in _disposables)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }
}