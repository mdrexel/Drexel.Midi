using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Midi.Internals;

/// <summary>
/// Defines a synchronous command behavior implementation.
/// </summary>
/// <inheritdoc cref="ICommandHandler{TOptions, THandler}"/>
internal abstract class SynchronousHandler<TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler> : ICommandHandler<TOptions, THandler>
    where THandler : ICommandHandler<TOptions, THandler>
{
    /// <inheritdoc/>
    public Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken) =>
        Task.Run(() => this.Handle(options), cancellationToken);

    /// <inheritdoc cref="ICommandHandler{TOptions, THandler}.HandleAsync(TOptions, CancellationToken)"/>
    protected abstract int Handle(TOptions options);
}