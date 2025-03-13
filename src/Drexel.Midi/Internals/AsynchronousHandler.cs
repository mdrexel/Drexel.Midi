using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Drexel.Midi.Internals;

/// <summary>
/// Defines an asynchronous command behavior implementation.
/// </summary>
/// <inheritdoc cref="ICommandHandler{TOptions, THandler}"/>
internal abstract class AsynchronousHandler<TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler> : ICommandHandler<TOptions, THandler>
    where THandler : ICommandHandler<TOptions, THandler>
{
    /// <inheritdoc/>
    public Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken) =>
        Task.Run(() => this.Handle(options, cancellationToken), cancellationToken);

    /// <inheritdoc cref="ICommandHandler{TOptions, THandler}.HandleAsync(TOptions, CancellationToken)"/>
    protected abstract int Handle(TOptions options, CancellationToken cancellationToken);
}