using System;
using System.Collections.Generic;
using System.Linq;
using Nito.Comparers;
using Nito.OptionParsing.LowLevel;
using Xunit;

// TODO: ignore case tests

namespace Nito.OptionParsing.UnitTests
{
    public class ShortOptionParserUnitTests
    {
        [Fact]
        public void Option_NotPassed_IsNotIncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'a' },
            };
            var result = ParseOptions(options, "and");
            Assert.DoesNotContain(new ParsedOption { Definition = options[0] }, result, CompareDefinitionsOnly);
        }

        [Fact]
        public void FlagOption_Passed_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x' },
            };
            var result = ParseOptions(options, "-x");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_ArgumentPassed_WithSpace_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "-x", "arg");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "arg" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_ArgumentPassed_WithColon_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "-x:arg");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "arg" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_ArgumentPassed_WithEquals_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "-x=arg");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "arg" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_ArgumentNotPassed_EndOfInput_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "-x");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_ArgumentNotPassed_FollowedByAnotherArgument_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Optional },
                new OptionDefinition { ShortName = 'y' },
            };
            var result = ParseOptions(options, "-x", "-y");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
                new ParsedOption { Definition = options[1] },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_ArgumentNotPassed_DoubleDash_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "-x", "--");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithRequiredArgument_WithSpace_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "-x", "bob");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "bob" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithRequiredArgument_WithColon_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "-x:bob");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "bob" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithRequiredArgument_WithEquals_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "-x=bob");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "bob" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void UnknownOption_Throws()
        {
            Assert.Throws<UnknownOptionException>(() => ParseOptions(null, "-x"));
        }

        [Fact]
        public void OptionRequiresArgument_ParmaeterWithoutArgument_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x', Argument = OptionArgument.Required },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "-x"));
        }

        [Fact]
        public void OptionWithoutArgument_PassedArgumentWithColon_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x' },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "-x:bob"));
        }

        [Fact]
        public void OptionWithoutArgument_PassedArgumentWithEquals_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'x' },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "-x=bob"));
        }

        [Fact]
        public void SingleDash_Throws()
        {
            Assert.Throws<InvalidParameterException>(() => ParseOptions(null, "-"));
        }

        private static List<ParsedOption> ParseOptions(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCulture, definitions ?? new OptionDefinition[0], commandLine).ToList();
        }

        private static readonly IEqualityComparer<ParsedOption> ParsedOptionComparer = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition).ThenEquateBy(x => x.Argument);

        private static readonly IEqualityComparer<ParsedOption> CompareDefinitionsOnly = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition);
    }
}
