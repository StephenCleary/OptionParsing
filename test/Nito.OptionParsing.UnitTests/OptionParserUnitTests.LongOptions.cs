using System;
using System.Collections.Generic;
using System.Linq;
using Nito.Comparers;
using Nito.OptionParsing.LowLevel;
using Xunit;

namespace Nito.OptionParsing.UnitTests
{
    public class OptionParserUnitTests_LongOptions
    {
        [Fact]
        public void Option_NotPassed_NotIncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "and" },
            };
            var result = ParseOptions(options, "and");
            Assert.DoesNotContain(new ParsedOption { Definition = options[0] }, result, CompareDefinitionsOnly);
        }

        [Fact]
        public void FlagOption_Passed_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option" },
            };
            var result = ParseOptions(options, "--option");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void FlagOption_PassedWrongCaseWhenCaseInsensitive_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option" },
            };
            var result = ParseOptionsIgnoringCase(options, "--Option");
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
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "--option", "arg");
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
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "--option:arg");
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
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "--option=arg");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "arg" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_EmptyArgument_WithEquals_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "--option=");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithOptionalArgument_ArgumentNotPassed_EndOfInput_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "--option");
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
                new OptionDefinition { LongName = "first", Argument = OptionArgument.Optional },
                new OptionDefinition { LongName = "second" },
            };
            var result = ParseOptions(options, "--first", "--second");
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
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "--option", "--");
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
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "--option", "bob");
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
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "--option:bob");
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
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "--option=bob");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "bob" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithRequiredArgument_WithSpace_ArgumentStartsWithDash_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "--option", "-3");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "-3" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionWithRequiredArgument_WithSpace_ArgumentStartsWithSlash_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "--option", "/src");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "/src" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void UnknownOption_Throws()
        {
            Assert.Throws<UnknownOptionException>(() => ParseOptions(null, "--option"));
        }

        [Fact]
        public void Option_WrongCase_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option" },
            };
            Assert.Throws<UnknownOptionException>(() => ParseOptions(options, "--Option"));
        }

        [Fact]
        public void OptionRequiresArgument_ParmaeterWithoutArgument_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option", Argument = OptionArgument.Required },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "--option"));
        }

        [Fact]
        public void OptionWithoutArgument_PassedArgumentWithColon_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option" },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "--option:bob"));
        }

        [Fact]
        public void OptionWithoutArgument_PassedArgumentWithEquals_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "option" },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "--option=bob"));
        }

        private static List<ParsedOption> ParseOptions(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCulture, definitions ?? new OptionDefinition[0], commandLine).ToList();
        }

        private static List<ParsedOption> ParseOptionsIgnoringCase(IReadOnlyCollection<OptionDefinition> definitions, params string[] commandLine)
        {
            return new OptionParser(StringComparer.InvariantCultureIgnoreCase, definitions ?? new OptionDefinition[0], commandLine).ToList();
        }

        private static readonly IEqualityComparer<ParsedOption> ParsedOptionComparer = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition).ThenEquateBy(x => x.Argument);

        private static readonly IEqualityComparer<ParsedOption> CompareDefinitionsOnly = EqualityComparerBuilder.For<ParsedOption>()
            .EquateBy(x => x.Definition);
    }
}
