using System;
using System.Collections.Generic;
using System.Linq;
using Nito.Comparers;
using Nito.OptionParsing.LowLevel;
using Xunit;

namespace Nito.OptionParsing.UnitTests
{
    public class PositionalOptionParserUnitTests
    {
        [Fact]
        public void PositionalArguments()
        {
            var result = ParseOptions(null, "bob");
            Assert.Equal(new[]
            {
                new ParsedOption { Argument = "bob" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void EverthingAfterDoubleDash_IsPositionalArgument()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x' },
            };
            var result = ParseOptions(options, "-x", "--", "-x", "hi", "-", "--");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
                new ParsedOption { Argument = "-x" },
                new ParsedOption { Argument = "hi" },
                new ParsedOption { Argument = "-" },
                new ParsedOption { Argument = "--" },
            }, result, ParsedOptionComparer);
        }

        private static List<ParsedOption> ParseOptions(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCulture, definitions ?? new OptionDefinition[0], commandLine).ToList();
        }

        private static readonly IEqualityComparer<ParsedOption> ParsedOptionComparer = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition).ThenEquateBy(x => x.Argument);
    }
}
