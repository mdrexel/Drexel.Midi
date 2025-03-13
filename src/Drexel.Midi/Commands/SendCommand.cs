using System;
using System.CommandLine;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Midi.Internals;
using Spectre.Console;

namespace Drexel.Midi.Commands;

/// <summary>
/// Sends a MIDI message to a device.
/// </summary>
internal sealed class SendCommand : Command<SendCommand.Options, SendCommand.Handler>
{
    private static Option<int> PinOption { get; } =
        new(["--pin", "-p"], "The numeric pin number to get the value of.")
        {
            Arity = ArgumentArity.ExactlyOne,
            IsRequired = true,
        };

    /// <summary>
    /// Initializes a new instance of the <see cref="SendCommand"/> class.
    /// </summary>
    public SendCommand()
        : base("send", "Sends a MIDI message to a device.")
    {
        Add(PinOption);
    }

    /// <summary>
    /// The options associated with performing the command.
    /// </summary>
    public new sealed class Options
    {
    }

    /// <summary>
    /// The command implementation.
    /// </summary>
    /// <param name="console">
    /// The console to use.
    /// </param>
    public new sealed class Handler(IAnsiConsole console) : ICommandHandler<Options, Handler>
    {
        /// <inheritdoc/>
        public async Task<int> HandleAsync(Options options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ExitCode.Success;
        }
    }
}