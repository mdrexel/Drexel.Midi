using System;
using NAudio.Midi;

namespace Drexel.Midi.Internals
{
    internal sealed class MidiInFilter : IDisposable
    {
        private readonly MidiIn _device;
        private readonly int? _channel;
        private readonly EventHandler<MidiInMessageEventArgs> _onMessage;
        private readonly EventHandler<MidiInSysexMessageEventArgs> _onSysex;

        public MidiInFilter(
            MidiIn device,
            int? channel,
            EventHandler<MidiInMessageEventArgs> onMessage,
            EventHandler<MidiInSysexMessageEventArgs> onSysex)
        {
            _device = device;
            _channel = channel;

            if (channel is null)
            {
                _onMessage = onMessage;
            }
            else
            {
                _onMessage =
                    (obj, e) =>
                    {
                        if (e.MidiEvent.Channel == channel.Value)
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
