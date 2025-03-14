using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Drexel.Midi.Internals
{
    internal sealed class MidiChannels : IReadOnlyList<MidiChannels.Channel>, IEquatable<MidiChannels>
    {
        private readonly Channel[] _channels;

        public MidiChannels(IReadOnlyList<int> channels)
        {
            _channels = new Channel[16];
            for (int index = 0; index < _channels.Length; index++)
            {
                _channels[index] = new(index + 1) { Enabled = channels.Contains(index + 1) };
            }
        }

        public MidiChannels(Range range)
        {
            _channels = new Channel[16];
            for (int index = 0; index < range.Start.Value - 1; index++)
            {
                _channels[index] = new(index + 1) { Enabled = false };
            }

            for (int index = range.Start.Value - 1; index < range.End.Value - 1; index++)
            {
                _channels[index] = new(index + 1) { Enabled = true };
            }

            for (int index = range.End.Value - 1; index < 16; index++)
            {
                _channels[index] = new(index + 1) { Enabled = false };
            }
        }

        public static MidiChannels All { get; } = new(1..17);

        public int Count => _channels.Length;

        public Channel this[int index] => _channels[index];

        public bool Contains(int channel) => _channels[channel - 1].Enabled;

        public bool Contains(Index index) => Contains(index.Value - 1);

        public bool Contains(Range range)
        {
            for (int counter = range.Start.Value - 1; counter < range.End.Value - 1; counter++)
            {
                if (!Contains(counter))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Contains(params IEnumerable<Index> indices) => indices.All(this.Contains);

        public bool Contains(params IEnumerable<Range> ranges) => ranges.All(this.Contains);

        public override bool Equals(object? obj) => this.Equals(obj as MidiChannels);

        public bool Equals(MidiChannels? other)
        {
            if (other is null)
            {
                return false;
            }
            else if (this.Count != other.Count)
            {
                // This should be literally impossible, but just in case...
                return false;
            }

            for (int counter = 0; counter < Count; counter++)
            {
                if (this[counter].Enabled != other[counter].Enabled)
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerator<Channel> GetEnumerator()
        {
            foreach (Channel channel in _channels)
            {
                yield return channel;
            }
        }

        public override int GetHashCode()
        {
            // Don't care about collisions, nothing should be taking the hash of this anyway. Just need this so that
            // we follow hashcode rules for equality.
            return 0;
        }

        public override string ToString() => $"[{string.Join(',', _channels.Where(x => x.Enabled).Select(x => x.Id))}]";

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public sealed class Channel
        {
            public Channel(int id)
            {
                if (id is < 1 or > 16)
                {
                    throw new ArgumentOutOfRangeException(nameof(id));
                }

                Id = id;
            }

            public int Id { get; }

            public required bool Enabled { get; init; }
        }
    }
}
