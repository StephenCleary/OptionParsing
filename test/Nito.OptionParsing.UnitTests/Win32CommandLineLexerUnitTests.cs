using System;
using System.Linq;
using Nito.OptionParsing.Lexing;
using Xunit;

namespace Nito.OptionParsing.UnitTests
{
    public class Win32CommandLineLexerUnitTests
    {
        [Theory]
        [InlineData(new string[0], "")]
        [InlineData(new[] { "test" }, "test")]
        [InlineData(new[] { "first", "second" }, "first second")]
        [InlineData(new[] { "first second" }, "\"first second\"")]
        [InlineData(new[] { "first second", "third" }, "\"first second\" third")]
        [InlineData(new[] { "contains \"quotes\"" }, "\"contains \\\"quotes\\\"\"")]
        public void RoundTrip(string[] expected, string input)
        {
            var lexed = Win32CommandLineLexer.Instance.Lex(input).ToList();
            Assert.Equal(expected, lexed);

            var combined = Win32CommandLineLexer.Instance.Escape(lexed);
            Assert.Equal(input, combined);
        }

        [Theory]
        [InlineData(new[] { "test" }, "  test  ")]
        [InlineData(new[] { "first\"second" }, "\"first\"\"second\"")]
        [InlineData(new[] { "test" }, "te\"s\"t")] // quotes in the middle of arguments (no whitespace)
        [InlineData(new[] { "bad" }, "ba\"d")] // unclosed quote
        [InlineData(new[] { "foo\"\"\"\"\"bar" }, "foo\"\"\"\"\"\"\"\"\"\"\"\"bar")]
        [InlineData(new[] { "" }, "\"")] // just a quote!
        [InlineData(new[] { "\"" }, "\"\\\"")] // just a backslash in quotes
        [InlineData(new[] { "\\" }, "\"\\\\\"")] // escaped backslash in quotes
        public void Lex_NoRoundTrip(string[] expected, string input)
        {
            var lexed = Win32CommandLineLexer.Instance.Lex(input).ToList();
            Assert.Equal(expected, lexed);
        }

        [Theory]
        [InlineData("\" \\\"", new[] { " \\" })]
        public void Escape_NoRoundTrip(string expected, string[] input)
        {
            var combined = Win32CommandLineLexer.Instance.Escape(input);
            Assert.Equal(expected, combined);
        }

        [Fact]
        public void Lex_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Win32CommandLineLexer.Instance.Lex(null).ToList());
        }

        [Fact]
        public void Escape_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => Win32CommandLineLexer.Instance.Escape(null));
        }
    }
}
