using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Midi;

namespace Drexel.Midi.Internals;

internal static class Utilities
{
    public static IReadOnlyList<(MidiIn Device, MidiChannels Channels)> ParseIdentifiersIn(IReadOnlyList<string> identifiers) =>
        identifiers
            .Select(
                x =>
                {
                    (int deviceId, MidiChannels channels) = ParseInputDevice(x);
                    return (new MidiIn(deviceId), channels);
                })
            .ToArray();

    public static IReadOnlyList<(MidiOut Device, MidiChannels Channels)> ParseIdentifiersOut(IReadOnlyList<string> identifiers) =>
        identifiers
            .Select(
                x =>
                {
                    (int deviceId, MidiChannels channels) = ParseOutputDevice(x);
                    return (new MidiOut(deviceId), channels);
                })
            .ToArray();

    public static IReadOnlyList<MidiInFilter> GetMidiIn(
        IReadOnlyList<string> identifiers,
        EventHandler<MidiInMessageEventArgs> onMessage,
        EventHandler<MidiInSysexMessageEventArgs> onSysex)
    {
        return ParseIdentifiersIn(identifiers).Select(x => new MidiInFilter(x.Device, x.Channels, onMessage, onSysex)).ToArray();
    }

    public static IReadOnlyList<MidiOutFilter> GetMidiOut(IReadOnlyList<string> identifiers)
    {
        return ParseIdentifiersOut(identifiers).Select(x => new MidiOutFilter(x.Device, x.Channels)).ToArray();
    }

    public static (int, MidiChannels) ParseInputDevice(string identifier)
    {
        string[] parts = identifier.Split(':');
        if (parts.Length > 2)
        {
            throw new ArgumentException($"The specified identifier is not of a recognized format.  Identifier: {identifier}", nameof(identifier));
        }

        int id = ParseDevice(parts[0]);
        if (parts.Length == 1)
        {
            return (id, MidiChannels.All);
        }
        else
        {
            return (id, ParseChannels(identifier, parts[1]));
        }


        static int ParseDevice(string identifier)
        {
            if (int.TryParse(identifier, out int id))
            {
                _ = MidiIn.DeviceInfo(id);
                return id;
            }

            for (int counter = 0; counter < MidiIn.NumberOfDevices; counter++)
            {
                MidiInCapabilities device = MidiIn.DeviceInfo(counter);
                if (StringComparer.Ordinal.Equals(identifier, device.ProductName))
                {
                    return counter;
                }
            }

            throw new ArgumentException($"The specified device ID or name was not recognized. Identifier: {identifier}", nameof(identifier));
        }
    }

    public static (int, MidiChannels) ParseOutputDevice(string identifier)
    {
        string[] parts = identifier.Split(':');
        if (parts.Length > 2)
        {
            throw new ArgumentException($"The specified identifier is not of a recognized format. Identifier: {identifier}", nameof(identifier));
        }

        int id = ParseDevice(parts[0]);
        if (parts.Length == 1)
        {
            return (id, MidiChannels.All);
        }
        else
        {
            return (id, ParseChannels(identifier, parts[1]));
        }

        static int ParseDevice(string identifier)
        {
            if (int.TryParse(identifier, out int id))
            {
                _ = MidiOut.DeviceInfo(id);
                return id;
            }

            for (int counter = 0; counter < MidiOut.NumberOfDevices; counter++)
            {
                MidiOutCapabilities device = MidiOut.DeviceInfo(counter);
                if (StringComparer.Ordinal.Equals(identifier, device.ProductName))
                {
                    return counter;
                }
            }

            throw new ArgumentException($"The specified device ID or name was not recognized. Identifier: {identifier}", nameof(identifier));
        }
    }

    internal static MidiChannels ParseChannels(string identifier, string channels)
    {
        if (int.TryParse(channels, out int channel))
        {
            if (channel is < 1 or > 16)
            {
                throw new ArgumentException($"The specified channel is out of range. Identifier: {identifier}", nameof(identifier));
            }

            return new MidiChannels([channel]);
        }

        string[] range = channels.Split('-');
        if (range.Length == 2 && int.TryParse(range[0], out int start) && int.TryParse(range[1], out int end))
        {
            return new MidiChannels(start..(end + 1));
        }

        throw new ArgumentException($"The specified identifier is not of a recognized format. Identifier: {identifier}", nameof(identifier));
    }
}