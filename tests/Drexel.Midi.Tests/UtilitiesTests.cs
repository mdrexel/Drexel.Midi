using System.Collections.Generic;
using Drexel.Midi.Internals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Drexel.Midi.Tests
{
    [TestClass]
    public sealed class UtilitiesTests
    {
        public static IEnumerable<object[]> ParseChannelsCases { get; } =
            new object[][]
            {
                ["1", new MidiChannels([1])],
                ["2", new MidiChannels([2])],
                ["3-5", new MidiChannels([3, 4, 5])],
            };

        [DataTestMethod]
        [DynamicData(nameof(ParseChannelsCases))]
        internal void ParseChannels_Succeeds(string channels, MidiChannels expected)
        {
            MidiChannels actual = Utilities.ParseChannels("dontCare", channels);

            Assert.AreEqual(expected, actual);
        }
    }
}
