using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Midi;

namespace Drexel.Midi.Internals
{
    internal class MidiOutFilter : IDisposable
    {
        private readonly MidiOut _device;
        private readonly MidiChannels.Channel[] _channels;

        public MidiOutFilter(MidiOut device, MidiChannels channels)
        {
            _device = device;
            _channels = channels.Where(x => x.Enabled).ToArray();
        }

        public void Dispose()
        {
            _device.Dispose();
        }

        public IEnumerable<MidiEvent> Send(MidiEvent message)
        {
            MidiEvent[] events = new MidiEvent[_channels.Length];
            for (int counter = 0; counter < _channels.Length; counter++)
            {
                MidiChannels.Channel channel = _channels[counter];

                MidiEvent clone = message.Clone();
                clone.Channel = channel.Id;

                events[counter] = clone;
                _device.Send(clone.GetAsShortMessage());
            }

            return events;
        }
    }
}
