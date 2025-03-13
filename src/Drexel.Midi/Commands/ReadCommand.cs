using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using Drexel.Midi.Internals;
using NAudio.Midi;
using Spectre.Console;

namespace Drexel.Midi.Commands;

/// <summary>
/// Reads MIDI message(s) from a device.
/// </summary>
internal sealed class ReadCommand : Command<ReadCommand.Options, ReadCommand.Handler>
{
    private static Option<string[]> FromOption { get; } =
        new(["--from", "-f"], "A device name or ID to read from, of the form `id{:channel}`.")
        {
            Arity = ArgumentArity.OneOrMore,
            IsRequired = true,
        };

    private static Option<int> NumberOption { get; } =
        new(["--number", "-n"], () => -1, "The number of MIDI messages to read, or a negative value to read indefinitely.")
        {
            Arity = ArgumentArity.ZeroOrOne,
        };

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadCommand"/> class.
    /// </summary>
    public ReadCommand()
        : base("read", "Reads MIDI message(s) from a device.")
    {
        Add(FromOption);
        Add(NumberOption);
    }

    /// <summary>
    /// The options associated with performing the command.
    /// </summary>
    public new sealed class Options
    {
        public required IReadOnlyList<string> From { get; init; }

        public required int Number { get; init; }
    }

    /// <summary>
    /// The command implementation.
    /// </summary>
    /// <param name="console">
    /// The console to use.
    /// </param>
    public new sealed class Handler(IAnsiConsole console) : AsynchronousHandler<Options, Handler>
    {
        /// <inheritdoc/>
        protected override int Handle(Options options, CancellationToken cancellationToken)
        {
            using ICountdown messageLimit = Countdown.Create(options.Number);
            IReadOnlyList<MidiInFilter> listeners = Utilities.GetMidiIn(options.From, MessageReceived, SysexMessageReceived);
            using CompositeDisposable token = new(listeners);

            foreach (MidiInFilter listener in listeners)
            {
                listener.Start();
            }

            try
            {
                messageLimit.Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Consider a premature cancellation to mean the user is no longer interested, and treat it as success.
            }
            finally
            {
                foreach (MidiInFilter listener in listeners)
                {
                    listener.Stop();
                }
            }

            return ExitCode.Success;

            void MessageReceived(object? sender, MidiInMessageEventArgs args)
            {
                console.WriteLine($"{args.Timestamp}: {args.MidiEvent}");
                messageLimit.Signal();
            }

            void SysexMessageReceived(object? sender, MidiInSysexMessageEventArgs args)
            {
                console.WriteLine($"{args.Timestamp}: SysEx Byte Count = {args.SysexBytes.Length}");
                messageLimit.Signal();
            }
        }
    }
}