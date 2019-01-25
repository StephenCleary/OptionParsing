using System;
using System.Collections.Generic;
using System.Text;
using Nito.OptionParsing.Converters;
using Xunit;
using static Nito.OptionParsing.UnitTests.Utility.OptionParsing;
// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Nito.OptionParsing.UnitTests
{
    public class CommandLineOptionsParserInvalidUnitTests
    {
        private sealed class OptionsSameLongName : CommandLineOptionsBase
        {
            [Option("first", 'f')] public string First { get; set; }
            [Option("first", 's')] public string Second { get; set; }
        }

        [Fact]
        public void Options_SameLongName_throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<OptionsSameLongName>());

        private sealed class OptionsSameShortName : CommandLineOptionsBase
        {
            [Option("first", 'f')] public string First { get; set; }
            [Option("second", 'f')] public string Second { get; set; }
        }

        [Fact]
        public void Options_SameShortName_throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<OptionsSameShortName>());

        private sealed class OptionsWithoutPositionalArguments : ICommandLineOptions
        {
            public CommandLineOptionsSettings CommandLineOptionsSettings => null;
            public void Validate() { }
            public void Done() { }
        }

        [Fact]
        public void Options_WithoutPositionalArguments_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<OptionsWithoutPositionalArguments>());

        private sealed class OptionPresentNoMatch : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.Optional)] public string SimpleOption { get; set; }
            [OptionPresent('z')] public bool SimpleOptionPresent { get; set; }
        }

        [Fact]
        public void OptionPresent_NoMatch_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<OptionPresentNoMatch>());

        private sealed class OptionPresentNonBoolean : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.Optional)] public string SimpleOption { get; set; }
            [OptionPresent('a')] public string SimpleOptionPresent { get; set; }
        }

        [Fact]
        public void OptionPresent_NonBoolean_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<OptionPresentNonBoolean>());

        private sealed class OptionWithoutArgumentNonBoolean : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.None)] public string FlagOption { get; set; }
        }

        [Fact]
        public void OptionWithoutArgument_NonBoolean_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<OptionWithoutArgumentNonBoolean>());

        private sealed class DuplicatePositionalArgument : CommandLineOptionsBase
        {
            [PositionalArgument(0)] public string First { get; set; }
            [PositionalArgument(0)] public string Second { get; set; }
        }

        [Fact]
        public void DuplicatePositionalArgument_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<DuplicatePositionalArgument>());

        private sealed class PositionalArgumentGap : CommandLineOptionsBase
        {
            [PositionalArgument(0)] public string First { get; set; }
            [PositionalArgument(2)] public string Third { get; set; }
        }

        [Fact]
        public void PositionalArgumentGap_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<PositionalArgumentGap>());

        private sealed class MultiplePositionalArgumentsAttributes : CommandLineOptionsBase
        {
            [PositionalArguments] public List<string> More { get; set; } = new List<string>();
        }

        [Fact]
        public void MultiplePositionalArgumentsAttributes_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<MultiplePositionalArgumentsAttributes>());

        private sealed class PositionalArgumentsAttributeWrongType : ICommandLineOptions
        {
            [PositionalArguments] public string More { get; set; }
            public CommandLineOptionsSettings CommandLineOptionsSettings => null;
            public void Validate() { }
            public void Done() { }
        }

        [Fact]
        public void PositionalArgumentsAttribute_WrongType_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<PositionalArgumentsAttributeWrongType>());

        private sealed class PositionalArgumentsAttributeWeirdType : ICommandLineOptions
        {
            public class MyList
            {
                public void Add(string val) { }
                public void Add(int val) { }
            }
            [PositionalArguments] public MyList More { get; set; } = new MyList();
            public CommandLineOptionsSettings CommandLineOptionsSettings => null;
            public void Validate() { }
            public void Done() { }
        }

        [Fact]
        public void PositionalArgumentsAttribute_WeirdType_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<PositionalArgumentsAttributeWeirdType>());

        private sealed class CustomTypeWithoutConverter : CommandLineOptionsBase
        {
            public sealed class MyInt { public int Value { get; set; } }
            [Option("level", 'l')] public MyInt SimpleOption { get; set; }
        }

        [Fact]
        public void CustomType_WithoutConverter_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<CustomTypeWithoutConverter>());

        private sealed class ExplicitCustomConverterWithoutInterface : CommandLineOptionsBase
        {
            public sealed class MyInt { public int Value { get; set; } }

            private sealed class MyIntConverter // : IOptionArgumentValueConverter
            {
                public bool CanConvert(Type type)
                {
                     Assert.True(false);
                     return false;
                }

                public object TryConvert(Type type, string text)
                {
                    Assert.True(false);
                    return null;
                }
            }

            [Option("level", Converter = typeof(MyIntConverter))] public MyInt Level { get; set; }
        }

        [Fact]
        public void ExplicitCustomConverter_WithoutInterface_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<ExplicitCustomConverterWithoutInterface>());

        private sealed class ExplicitCustomConverterReturnsFalseFromCanConvert : CommandLineOptionsBase
        {
            public sealed class MyInt { public int Value { get; set; } }

            private sealed class MyIntConverter : IOptionArgumentValueConverter
            {
                public bool CanConvert(Type type) => false;
                public object TryConvert(Type type, string text) => new MyInt { Value = 7 };
            }

            [Option("level", Converter = typeof(MyIntConverter))] public MyInt Level { get; set; }
        }

        [Fact]
        public void ExplicitCustomConverter_ReturnsFalseFromCanConvert_Throws() =>
            Assert.Throws<InvalidOperationException>(() => Parse<ExplicitCustomConverterReturnsFalseFromCanConvert>());
    }
}
