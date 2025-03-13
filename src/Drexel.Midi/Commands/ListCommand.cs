using System.Collections.Generic;
using System.Linq;
using Drexel.Midi.Internals;
using NAudio.Midi;
using Spectre.Console;

namespace Drexel.Midi.Commands;

/// <summary>
/// Lists attached MIDI devices.
/// </summary>
internal sealed class ListCommand : Command<ListCommand.Options, ListCommand.Handler>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListCommand"/> class.
    /// </summary>
    public ListCommand()
        : base("list", "Lists attached MIDI devices.")
    {
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
    public new sealed class Handler(IAnsiConsole console) : SynchronousHandler<Options, Handler>
    {
        protected override int Handle(Options options)
        {
            IReadOnlyList<MidiOutCapabilities> outputDevices = Enumerable
                .Range(0, MidiOut.NumberOfDevices)
                .Select(MidiOut.DeviceInfo)
                .ToArray();
            IReadOnlyList<MidiInCapabilities> inputDevices = Enumerable
                .Range(0, MidiIn.NumberOfDevices)
                .Select(MidiIn.DeviceInfo)
                .ToArray();

            console.WriteLine("Output devices:");
            foreach ((MidiOutCapabilities device, int index) in outputDevices.Select((x, i) => (x, i)))
            {
                console.WriteLine($"    {index}: {device.ProductName}");
            }

            console.WriteLine();
            console.WriteLine("Input devices:");
            foreach ((MidiInCapabilities device, int index) in inputDevices.Select((x, i) => (x, i)))
            {
                console.WriteLine($"    {index}: {device.ProductName}");
            }

            return ExitCode.Success;
        }
    }
}