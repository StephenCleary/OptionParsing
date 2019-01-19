using System;
using System.Collections.Generic;
using System.Linq;
using Nito.OptionParsing.LowLevel;
using Xunit;
using static Nito.OptionParsing.UnitTests.Utility.OptionParsing;

namespace Nito.OptionParsing.UnitTests
{
    public partial class OptionParserUnitTest
    {
        [Fact]
        public void Option_NotPassed_NotIncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'a', LongName = "and" },
            };
            var result = ParseOptions(options, "and");
            Assert.DoesNotContain(new ParsedOption { Definition = options[0] }, result, CompareDefinitionsOnly);
        }

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
                new OptionDefinition { ShortName = 'o', LongName = "option" },
            };
            var result = ParseOptions(options, "-o", "--", "-o", "hi", "-", "--", "--option", "/o", "/option");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
                new ParsedOption { Argument = "-o" },
                new ParsedOption { Argument = "hi" },
                new ParsedOption { Argument = "-" },
                new ParsedOption { Argument = "--" },
                new ParsedOption { Argument = "--option" },
                new ParsedOption { Argument = "/o" },
                new ParsedOption { Argument = "/option" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void SingleDash_Throws()
        {
            Assert.Throws<InvalidParameterException>(() => ParseOptions(null, "-"));
        }

        [Fact]
        public void SingleSlash_Throws()
        {
            Assert.Throws<InvalidParameterException>(() => ParseOptions(null, "/"));
        }

        [Fact]
        public void Option_WithoutName_Throws()
        {
            var options = new[]
            {
                new OptionDefinition(),
            };
            Assert.Throws<InvalidOperationException>(() => ParseOptions(options));
        }

        [Fact]
        public void OptionShortName_IsArgumentDelimiter_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = ':' },
            };
            Assert.Throws<InvalidOperationException>(() => ParseOptions(options));
        }

        [Fact]
        public void OptionLongName_ContainsArgumentDelimiter_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "a=b" },
            };
            Assert.Throws<InvalidOperationException>(() => ParseOptions(options));
        }

        [Fact]
        public void Options_WithSameShortName_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x' },
                new OptionDefinition { ShortName = 'x' },
            };
            Assert.Throws<InvalidOperationException>(() => ParseOptions(options));
        }

        [Fact]
        public void Options_WithSameLongName_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "and" },
                new OptionDefinition { LongName = "and" },
            };
            Assert.Throws<InvalidOperationException>(() => ParseOptions(options));
        }
    }
}
