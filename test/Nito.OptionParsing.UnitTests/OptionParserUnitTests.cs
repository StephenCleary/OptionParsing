using System;
using System.Collections.Generic;
using System.Linq;
using Nito.OptionParsing.LowLevel;
using Xunit;
using static Nito.OptionParsing.UnitTests.Utility.OptionParsing;

namespace Nito.OptionParsing.UnitTests
{
    public class OptionParserUnitTest
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
            var result = ParseOptions(null, "bob", "/o", "/usr/bin/", "/this:that", "/x=y");
            Assert.Equal(new[]
            {
                new ParsedOption { Argument = "bob" },
                new ParsedOption { Argument = "/o" },
                new ParsedOption { Argument = "/usr/bin/" },
                new ParsedOption { Argument = "/this:that" },
                new ParsedOption { Argument = "/x=y" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void Options_AfterPositionalArguments_WithParsingFlag_AreParsed()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'a', LongName = "and", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "0", "-a", "1");
            Assert.Equal(new[]
            {
                new ParsedOption { Argument = "0" },
                new ParsedOption { Definition = options[0], Argument = "1" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void Options_AfterPositionalArguments_WithoutParsingFlag_TreatedAsPositionalArguments()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'a', LongName = "and", Argument = OptionArgument.Optional },
            };
            var result = new OptionParser(StringComparer.InvariantCulture, options, new[] { "0", "-a", "1" }, false,
                parseOptionsAfterPositionalArguments: false).ToList();
            Assert.Equal(new[]
            {
                new ParsedOption { Argument = "0" },
                new ParsedOption { Argument = "-a" },
                new ParsedOption { Argument = "1" },
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
            Assert.Throws<InvalidParameterException>(() => ParseSlashOptions(null, "/"));
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
        public void Options_WithSameShortName_CaseInsensitive_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x' },
                new OptionDefinition { ShortName = 'X' },
            };
            ParseOptions(options);
            Assert.Throws<InvalidOperationException>(() => ParseOptionsIgnoringCase(options));
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

        [Fact]
        public void Options_WithSameLongName_CaseInsensitive_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "and" },
                new OptionDefinition { LongName = "And" },
            };
            ParseOptions(options);
            Assert.Throws<InvalidOperationException>(() => ParseOptionsIgnoringCase(options));
        }
    }
}
