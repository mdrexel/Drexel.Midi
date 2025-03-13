using System;
using NAudio.Midi;

namespace Drexel.Midi.Internals
{
    internal class MidiOutFilter : IDisposable
    {
        private readonly MidiOut _device;
        private readonly int? _channel;

        public MidiOutFilter(MidiOut device, int? channel)
        {
            _device = device;
            _channel = channel;
        }

        public void Dispose()
        {
            _device.Dispose();
        }

        public MidiEvent Send(MidiEvent message)
        {
            if (_channel is not null)
            {
                message = message.Clone();
                message.Channel = _channel.Value;
            }

            _device.Send(message.GetAsShortMessage());
            return message;
        }
    }
}
