using System;
using System.Collections.Generic;
using System.Linq;
using Nito.OptionParsing.LowLevel;
using Xunit;
using static Nito.OptionParsing.UnitTests.Utility.OptionParsing;

namespace Nito.OptionParsing.UnitTests
{
    public class OptionParserUnitTests_LongSlashOptions
    {
        [Fact]
        public void FlagOption_Passed_IncludedInOutput()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'o', LongName = "option" },
            };
            var result = ParseOptions(options, "/option");
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
                new OptionDefinition { ShortName = 'o', LongName = "option" },
            };
            var result = ParseOptionsIgnoringCase(options, "/Option");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "/option", "arg");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "/option:arg");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "/option=arg");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "/option=");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "/option");
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
                new OptionDefinition { ShortName = 'f', LongName = "first", Argument = OptionArgument.Optional },
                new OptionDefinition { ShortName = 's', LongName = "second" },
            };
            var result = ParseOptions(options, "/first", "/second");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Optional },
            };
            var result = ParseOptions(options, "/option", "--");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "/option", "bob");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "/option:bob");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "/option=bob");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "/option", "-3");
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
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Required },
            };
            var result = ParseOptions(options, "/option", "/src");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0], Argument = "/src" },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionNamedSlash_CanBeUsedWithDash()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "/" },
            };
            var result = ParseOptions(options, "--/");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void OptionNamedSlash_MayBeUsedWithSlash()
        {
            var options = new[]
            {
                new OptionDefinition { LongName = "/" },
            };
            var result = ParseOptions(options, "//");
            Assert.Equal(new[]
            {
                new ParsedOption { Definition = options[0] },
            }, result, ParsedOptionComparer);
        }

        [Fact]
        public void UnknownOption_Throws()
        {
            Assert.Throws<UnknownOptionException>(() => ParseOptions(null, "/option"));
        }

        [Fact]
        public void Option_WrongCase_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'o', LongName = "option" },
            };
            Assert.Throws<UnknownOptionException>(() => ParseOptions(options, "/Option"));
        }

        [Fact]
        public void OptionRequiresArgument_ParmaeterWithoutArgument_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'o', LongName = "option", Argument = OptionArgument.Required },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "/option"));
        }

        [Fact]
        public void OptionWithoutArgument_PassedArgumentWithColon_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'o', LongName = "option" },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "/option:bob"));
        }

        [Fact]
        public void OptionWithoutArgument_PassedArgumentWithEquals_Throws()
        {
            var options = new[]
            {
                new OptionDefinition { ShortName = 'o', LongName = "option" },
            };
            Assert.Throws<OptionArgumentException>(() => ParseOptions(options, "/option=bob"));
        }
    }
}
