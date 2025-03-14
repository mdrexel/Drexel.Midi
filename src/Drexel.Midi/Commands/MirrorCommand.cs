using System.Collections.Generic;
using System.CommandLine;
using System.Threading;
using Drexel.Midi.Internals;
using NAudio.Midi;
using Spectre.Console;

namespace Drexel.Midi.Commands;

/// <summary>
/// Mirrors MIDI input(s) to MIDI output(s).
/// </summary>
internal sealed class MirrorCommand : Command<MirrorCommand.Options, MirrorCommand.Handler>
{
    private static Option<string[]> FromOption { get; } =
        new(["--from", "-f"], "A device name or ID to read from, of the form `id{:channel}`.")
        {
            Arity = ArgumentArity.OneOrMore,
            IsRequired = true,
        };

    private static Option<string[]> ToOption { get; } =
        new(["--to", "-t"], "A device name or ID to send to, of the form `id{:channel}`.")
        {
            Arity = ArgumentArity.OneOrMore,
            IsRequired = true,
        };

    /// <summary>
    /// Initializes a new instance of the <see cref="MirrorCommand"/> class.
    /// </summary>
    public MirrorCommand()
        : base("mirror", "Mirrors a MIDI input(s) to MIDI output(s).")
    {
        Add(FromOption);
        Add(ToOption);
    }

    /// <summary>
    /// The options associated with performing the command.
    /// </summary>
    public new sealed class Options
    {
        public IReadOnlyList<string> From { get; init; }

        public IReadOnlyList<string> To { get; init; }
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
            IReadOnlyList<MidiOutFilter> to = Utilities.GetMidiOut(options.To);
            IReadOnlyList<MidiInFilter> from = Utilities.GetMidiIn(
                options.From,
                onMessage:
                    (obj, e) =>
                    {
                        console.WriteLine($"in:  {e.Timestamp}: {e.MidiEvent}");
                        foreach (MidiOutFilter @out in to)
                        {
                            foreach (MidiEvent sent in @out.Send(e.MidiEvent))
                            {
                                console.WriteLine($"out: {e.Timestamp}: {sent}");
                            }
                        }
                    },
                onSysex: (obj, e) => { });
            using CompositeDisposable token = new(from, to);

            foreach (MidiInFilter listener in from)
            {
                listener.Start();
            }

            ManualResetEventSlim complete = new();
            cancellationToken.Register(complete.Set);
            complete.Wait();

            foreach (MidiInFilter listener in from)
            {
                listener.Stop();
            }

            return ExitCode.Success;
        }
    }
}