using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Midi;

namespace Drexel.Midi.Internals;

internal static class Utilities
{
    public static IReadOnlyList<(MidiIn Device, int? Channel)> ParseIdentifiersIn(IReadOnlyList<string> identifiers) =>
        identifiers
            .Select(
                x =>
                {
                    (int deviceId, int? channel) = ParseInputDevice(x);
                    return (new MidiIn(deviceId), channel);
                })
            .ToArray();

    public static IReadOnlyList<(MidiOut Device, int? Channel)> ParseIdentifiersOut(IReadOnlyList<string> identifiers) =>
        identifiers
            .Select(
                x =>
                {
                    (int deviceId, int? channel) = ParseInputDevice(x);
                    return (new MidiOut(deviceId), channel);
                })
            .ToArray();

    public static IReadOnlyList<MidiInFilter> GetMidiIn(
        IReadOnlyList<string> identifiers,
        EventHandler<MidiInMessageEventArgs> onMessage,
        EventHandler<MidiInSysexMessageEventArgs> onSysex)
    {
        return ParseIdentifiersIn(identifiers).Select(x => new MidiInFilter(x.Device, x.Channel, onMessage, onSysex)).ToArray();
    }

    public static IReadOnlyList<MidiOutFilter> GetMidiOut(IReadOnlyList<string> identifiers)
    {
        return ParseIdentifiersOut(identifiers).Select(x => new MidiOutFilter(x.Device, x.Channel)).ToArray();
    }

    public static (int, int?) ParseInputDevice(string identifier)
    {
        IReadOnlyList<string> parts = identifier.Split(':');
        if (parts.Count > 2)
        {
            throw new ArgumentException($"The specified identifier is not of a recognized format.  Identifier: {identifier}", nameof(identifier));
        }

        int id = ParseDevice(parts[0]);
        if (parts.Count == 1)
        {
            return (id, null);
        }

        if (int.TryParse(parts[1], out int channel))
        {
            if (channel is <1 or >16)
            {
                throw new ArgumentException($"The specified channel is out of range. Identifier: {identifier}", nameof(identifier));
            }

            return (id, channel);
        }
        else
        {
            throw new ArgumentException($"The specified identifier is not of a recognized format.  Identifier: {identifier}", nameof(identifier));
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

    public static (int, int?) ParseOutputDevice(string identifier)
    {
        IReadOnlyList<string> parts = identifier.Split(':');
        if (parts.Count > 2)
        {
            throw new ArgumentException($"The specified identifier is not of a recognized format. Identifier: {identifier}", nameof(identifier));
        }

        int id = ParseDevice(parts[0]);
        if (parts.Count == 1)
        {
            return (id, null);
        }

        if (int.TryParse(parts[1], out int channel))
        {
            if (channel is < 1 or > 16)
            {
                throw new ArgumentException($"The specified channel is out of range. Identifier: {identifier}", nameof(identifier));
            }

            return (id, channel);
        }
        else
        {
            throw new ArgumentException($"The specified identifier is not of a recognized format. Identifier: {identifier}", nameof(identifier));
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
}