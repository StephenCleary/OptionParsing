using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using Xunit;
using static Nito.OptionParsing.UnitTests.Utility.OptionParsing;
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Nito.OptionParsing.UnitTests
{
    public class CommandLineOptionsParserUnitTests
    {
        private sealed class NoOptions : CommandLineOptionsBase
        {
        }

        [Fact]
        public void DefaultValidate_ThrowsOnAdditionalPositionalParameters()
        {
            Assert.Throws<UnknownOptionException>(() => Parse<NoOptions>("bob"));
        }

        private sealed class RequiredStringOption : CommandLineOptionsBase
        {
            [Option("and", 'a')] public string SimpleOption { get; set; }
        }

        [Fact]
        public void StringOption_RequiredParameterPassedAsShort_IsSet()
        {
            var result = Parse<RequiredStringOption>("-a", "test");
            Assert.Equal("test", result.SimpleOption);
        }

        [Fact]
        public void StringOption_RequiredParameterPassedAsLong_IsSet()
        {
            var result = Parse<RequiredStringOption>("--and", "13");
            Assert.Equal("13", result.SimpleOption);
        }

        [Fact]
        public void StringOption_RequiredParameterNotPassed_IsNull()
        {
            var result = Parse<RequiredStringOption>();
            Assert.Null(result.SimpleOption);
        }

        private sealed class RequiredStringOptionDefaultValue: CommandLineOptionsBase
        {
            [Option("and", 'a')] public string SimpleOption { get; set; } = "default";
        }

        [Fact]
        public void StringOptionWithDefault_RequiredParameterNotPassed_IsDefault()
        {
            var result = Parse<RequiredStringOptionDefaultValue>();
            Assert.Equal("default", result.SimpleOption);
        }

        [Fact]
        public void StringOptionWithDefault_RequiredParameterPassed_OverwritesDefault()
        {
            var result = Parse<RequiredStringOptionDefaultValue>("-a=bob");
            Assert.Equal("bob", result.SimpleOption);
        }

        private sealed class OptionalStringOptionDefaultValue : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.Optional)] public string SimpleOption { get; set; } = "default";
        }

        [Fact]
        public void StringOptionWithDefault_OptionNotPassed_IsDefault()
        {
            var result = Parse<OptionalStringOptionDefaultValue>();
            Assert.Equal("default", result.SimpleOption);
        }

        [Fact]
        public void StringOptionWithDefault_OptionalParameterNotPassed_IsDefault()
        {
            var result = Parse<OptionalStringOptionDefaultValue>("-a");
            Assert.Equal("default", result.SimpleOption);
        }

        [Fact]
        public void StringOptionWithDefault_OptionalParameterPassed_OverwritesDefault()
        {
            var result = Parse<OptionalStringOptionDefaultValue>("--and:bob");
            Assert.Equal("bob", result.SimpleOption);
        }

        private sealed class OptionPresentDefaultValue : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.Optional)] public string SimpleOption { get; set; } = "default";
            [OptionPresent("and")] public bool SimpleOptionPresent { get; set; }
        }

        [Fact]
        public void OptionPresent_OptionNotPassed_IsFalse()
        {
            var result = Parse<OptionPresentDefaultValue>();
            Assert.False(result.SimpleOptionPresent);
        }

        [Fact]
        public void OptionPresent_OptionalParameterPassed_IsTrue()
        {
            var result = Parse<OptionPresentDefaultValue>("-a=bob");
            Assert.True(result.SimpleOptionPresent);
        }

        [Fact]
        public void OptionPresent_OptionalParameterNotPassed_IsTrue()
        {
            var result = Parse<OptionPresentDefaultValue>("-a");
            Assert.True(result.SimpleOptionPresent);
        }

        private sealed class OptionPresentShortName : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.Optional)] public string SimpleOption { get; set; } = "default";
            [OptionPresent('a')] public bool SimpleOptionPresent { get; set; }
        }

        [Fact]
        public void LongOptionPresent_OptionalParameterNotPassed_IsTrue()
        {
            var result = Parse<OptionPresentShortName>("--and");
            Assert.True(result.SimpleOptionPresent);
        }

        private sealed class BuiltinConverter : CommandLineOptionsBase
        {
            [Option("level", 'l')] public int Level { get; set; }
        }

        [Fact]
        public void BuiltinConverter_Converts()
        {
            var result = Parse<BuiltinConverter>("-l", "13");
            Assert.Equal(13, result.Level);
        }

        private sealed class BuiltinNullableConverter : CommandLineOptionsBase
        {
            [Option("level", 'l')] public int? Level { get; set; }
        }

        [Fact]
        public void BuiltinNullableConverter_Converts()
        {
            var result = Parse<BuiltinNullableConverter>("-l", "13");
            Assert.Equal(13, result.Level);
        }
    }
}
