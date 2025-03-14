using System;
using NAudio.Midi;

namespace Drexel.Midi.Internals
{
    internal sealed class MidiInFilter : IDisposable
    {
        private readonly MidiIn _device;
        private readonly MidiChannels _channels;
        private readonly EventHandler<MidiInMessageEventArgs> _onMessage;
        private readonly EventHandler<MidiInSysexMessageEventArgs> _onSysex;

        public MidiInFilter(
            MidiIn device,
            MidiChannels channels,
            EventHandler<MidiInMessageEventArgs> onMessage,
            EventHandler<MidiInSysexMessageEventArgs> onSysex)
        {
            _device = device;
            _channels = channels;

            if (_channels == MidiChannels.All)
            {
                _onMessage = onMessage;
            }
            else
            {
                _onMessage =
                    (obj, e) =>
                    {
                        if (_channels.Contains(e.MidiEvent.Channel))
                        {
                            onMessage.Invoke(obj, e);
                        }
                    };
            }

            _onSysex = onSysex;
        }

        public void Start()
        {
            _device.MessageReceived += _onMessage;
            _device.SysexMessageReceived += _onSysex;
            _device.Start();
        }

        public void Stop()
        {
            _device.Stop();
            _device.MessageReceived -= _onMessage;
            _device.SysexMessageReceived -= _onSysex;
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
