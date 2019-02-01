using System;
using System.Collections.Generic;
using System.Text;
using Nito.OptionParsing.Lexing;
using Xunit;
using static Nito.OptionParsing.UnitTests.Utility.OptionParsing;
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Nito.OptionParsing.UnitTests
{
    public class CommandLineOptionsParserMultipleValueUnitTests
    {
        private sealed class OverwriteOptions : CommandLineOptionsBase
        {
            [Option("level", 'l')]
            public int Level { get; set; }
        }

        [Fact]
        public void DefaultBehavior_IsOverwrite()
        {
            var options = Parse<OverwriteOptions>("-l=7", "-l=3", "-l=13");
            Assert.Equal(13, options.Level);
        }

        private sealed class AccumulateOptions : CommandLineOptionsBase
        {
            public List<int> Levels { get; } = new List<int>();

            [Option("level", 'l')]
            private int Level { set => Levels.Add(value); }
        }

        [Fact]
        public void AccumulateExample()
        {
            var options = Parse<AccumulateOptions>("-l=7", "-l=3", "-l=13");
            Assert.Equal(new[] { 7, 3, 13 }, options.Levels);
        }

        private sealed class PreventOptions : CommandLineOptionsBase
        {
            private int? _level;

            [Option("level", 'l')]
            public int? Level
            {
                get => _level;
                set
                {
                    if (_level == null)
                        _level = value;
                    else
                        throw new InvalidOperationException("Level passed multiple times");
                }
            }
        }

        [Fact]
        public void PreventExample()
        {
            Assert.ThrowsAny<OptionParsingException>(() => Parse<PreventOptions>("-l=7", "-l=3"));

            var options = Parse<PreventOptions>("-l=7");
            Assert.Equal(7, options.Level);
        }
    }
}
