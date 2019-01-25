using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using Nito.OptionParsing.Converters;
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

        private sealed class RequiredStringOptionDefaultValue : CommandLineOptionsBase
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

        private sealed class OptionWithoutArgument : CommandLineOptionsBase
        {
            [Option("and", 'a', OptionArgument.None)] public bool FlagOption { get; set; }
        }

        [Fact]
        public void OptionWithoutArgument_OptionNotPassed_IsFalse()
        {
            var result = Parse<OptionWithoutArgument>();
            Assert.False(result.FlagOption);
        }

        [Fact]
        public void OptionWithoutArgument_OptionPassed_IsTrue()
        {
            var result = Parse<OptionWithoutArgument>("-a");
            Assert.True(result.FlagOption);
        }

        private sealed class PositionalArgumentOptions : CommandLineOptionsBase
        {
            [PositionalArgument(1)] public int Second { get; set; }
            [PositionalArgument(0)] public string First { get; set; }
        }

        [Fact]
        public void PositionalArguments_AreUsed()
        {
            var result = Parse<PositionalArgumentOptions>("a", "13");
            Assert.Equal("a", result.First);
            Assert.Equal(13, result.Second);
        }

        [Fact]
        public void PositionalArgument_UnparseableValue_Throws()
        {
            Assert.Throws<OptionArgumentException>(() => Parse<PositionalArgumentOptions>("x", "y"));
        }

        private sealed class PositionalArgumentsOptions : CommandLineOptionsBase
        {
            public override void Validate() { }
        }

        [Fact]
        public void PositionalArguments_Parsed()
        {
            var result = Parse<PositionalArgumentsOptions>("this", "that");
            Assert.Equal(new[] { "this", "that" }, result.AdditionalArguments);
        }

        private sealed class ParsedPositionalArgumentsOptions : ICommandLineOptions
        {
            [PositionalArguments] public List<int> More { get; set; } = new List<int>();
            public void Validate() { }
        }

        [Fact]
        public void ParsedPositionalArguments_Parsed()
        {
            var result = Parse<ParsedPositionalArgumentsOptions>("13", "7");
            Assert.Equal(new[] { 13, 7 }, result.More);
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

        [Fact]
        public void BuiltinConverter_UnparseableValue_Throws()
        {
            Assert.Throws<OptionArgumentException>(() => Parse<BuiltinConverter>("-l", "bob"));
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

        [Fact]
        public void BuiltinNullableConverter_UnparseableValue_Throws()
        {
            Assert.Throws<OptionArgumentException>(() => Parse<BuiltinNullableConverter>("-l", "bob"));
        }

        private sealed class BuiltinEnumConverter : CommandLineOptionsBase
        {
            [Flags]
            public enum Animal : byte
            {
                None,
                Dog,
                Cat,
                Mongoose
            }
            [Option("animal")] public Animal SelectedAnimal { get; set; }
        }

        [Fact]
        public void BuiltinEnumConverter_NoValues()
        {
            var result = Parse<BuiltinEnumConverter>();
            Assert.Equal(BuiltinEnumConverter.Animal.None, result.SelectedAnimal);
        }

        [Fact]
        public void BuiltinEnumConverter_SingleValue()
        {
            var result = Parse<BuiltinEnumConverter>("--animal=Mongoose");
            Assert.Equal(BuiltinEnumConverter.Animal.Mongoose, result.SelectedAnimal);
        }

        [Fact]
        public void BuiltinEnumConverter_MultipleValues()
        {
            var result = Parse<BuiltinEnumConverter>("--animal:Cat,Dog");
            Assert.Equal(BuiltinEnumConverter.Animal.Cat | BuiltinEnumConverter.Animal.Dog, result.SelectedAnimal);
        }

        [Fact]
        public void BuiltinEnumConverter_Numeric()
        {
            var result = Parse<BuiltinEnumConverter>("--animal:1");
            Assert.Equal(BuiltinEnumConverter.Animal.Dog, result.SelectedAnimal);
        }

        [Fact]
        public void BuiltinEnumConverter_Numeric_OutOfUnderlyingTypeRange()
        {
            Assert.Throws<OptionArgumentException>(() => Parse<BuiltinEnumConverter>("--animal:1000"));
        }

        [Fact]
        public void BuiltinEnumConverter_IsCaseInsensitive()
        {
            var result = Parse<BuiltinEnumConverter>("--animal", "mongoose");
            Assert.Equal(BuiltinEnumConverter.Animal.Mongoose, result.SelectedAnimal);
        }

        [Fact]
        public void BuiltinEnumConverter_UnparseableValue_Throws()
        {
            Assert.Throws<OptionArgumentException>(() => Parse<BuiltinEnumConverter>("--animal", "bob"));
        }

        private sealed class MyInt { public int Value { get; set; } }

        private sealed class MyIntConverter : IOptionArgumentValueConverter
        {
            public bool CanConvert(Type type) => type == typeof(MyInt);
            public object TryConvert(Type type, string text)
            {
                Assert.Equal(typeof(MyInt), type);
                return !int.TryParse(text, out var value) ? null : new MyInt { Value = value };
            }
        }

        private sealed class ExplicitCustomConverter : CommandLineOptionsBase
        {
            [Option("level", Converter = typeof(MyIntConverter))] public MyInt Level { get; set; }
        }

        [Fact]
        public void ExplicitCustomConverter_IsUsed()
        {
            var result = Parse<ExplicitCustomConverter>("--level:13");
            Assert.Equal(13, result.Level.Value);
        }

        [Fact]
        public void ExplicitCustomConverter_FailsToParse_Throws()
        {
            Assert.Throws<OptionArgumentException>(() => Parse<ExplicitCustomConverter>("--level:bob"));
        }

        private sealed class ImplicitCustomConverter : CommandLineOptionsBase
        {
            [Option("level")] public MyInt Level { get; set; }
        }

        [Fact]
        public void ImplicitCustomConverter_IsUsed()
        {
            var result = Parse<ImplicitCustomConverter>(new CommandLineOptionsSettings
            {
                OptionArgumentValueConverters = new List<IOptionArgumentValueConverter> { new MyIntConverter() }
            },"--level:5");
            Assert.Equal(5, result.Level.Value);
        }

        private interface IInterfaceOption
        {
            [Option("level", 'l')] int Level { get; set; }
        }

        private sealed class OptionBehindExplicitInterface : CommandLineOptionsBase, IInterfaceOption
        {
            int IInterfaceOption.Level { get; set; }
        }

        [Fact]
        public void OptionBehindExplicitInterface_IsNotUsed()
        {
            Assert.Throws<UnknownOptionException>(() => Parse<OptionBehindExplicitInterface>("-l=7"));
        }

        private interface IInterfaceOptionValue
        {
            int Level { get; set; }
        }
        
        private sealed class OptionOnExplicitInterface : CommandLineOptionsBase, IInterfaceOptionValue
        {
            [Option("level", 'l')] int IInterfaceOptionValue.Level { get; set; }
        }

        [Fact]
        public void OptionOnExplicitInterface_IsUsed()
        {
            var result = Parse<OptionOnExplicitInterface>("-l=7");
            Assert.Equal(7, ((IInterfaceOptionValue)result).Level);
        }

        private sealed class OptionBehindAndOnExplicitInterface : CommandLineOptionsBase, IInterfaceOption
        {
            [Option("another", 'a')] int IInterfaceOption.Level { get; set; }
        }

        [Fact]
        public void OptionBehindAndOnExplicitInterface_UsesOnlyMoreSpecificOptionDefinition()
        {
            var result = Parse<OptionBehindAndOnExplicitInterface>("-a=7");
            Assert.Equal(7, ((IInterfaceOption) result).Level);
            Assert.Throws<UnknownOptionException>(() => Parse<OptionBehindAndOnExplicitInterface>("-l=7"));
        }

        // Use case example: command structure, with shared common options base type
        // Use case example: multiple option sets for different inputs
        // Use case example: regular options mixed in with positional arguments
        // - Can we do this by default?
        //   - Perhaps by splitting up "Done" signal from "Validate" request?
        //     - And for those not using Base, how easy is it to implement this "Done" signal?
        // base.AdditionalArguments should be hidden behind an interface so the end-user consuming is clean.
    }
}
