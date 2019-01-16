using System;
using System.Linq;
using Xunit;

namespace Nito.OptionParsing.UnitTests
{
    public class QuoteBasedCommandLineLexerUnitTests
    {
        [Theory]
        [InlineData(new string[0], "")]
        [InlineData(new[] { "test" }, "test")]
        [InlineData(new[] { "first", "second" }, "first second")]
        [InlineData(new[] { "first second" }, "\"first second\"")]
        [InlineData(new[] { "first second", "third" }, "\"first second\" third")]
        [InlineData(new[] { "contains \"quotes\"" }, "\"contains \"\"quotes\"\"\"")]
        [InlineData(new[] { "foo\"\"\"\"\"\"bar" }, "\"foo\"\"\"\"\"\"\"\"\"\"\"\"bar\"")]
        public void RoundTrip(string[] expected, string input)
        {
            var lexed = QuoteBasedCommandLineLexer.Instance.Lex(input).ToList();
            Assert.Equal(expected, lexed);

            var combined = QuoteBasedCommandLineLexer.Instance.Escape(lexed);
            Assert.Equal(input, combined);
        }

        [Theory]
        [InlineData(new[] { "test" }, "  test  ")]
        [InlineData(new[] { "test" }, "te\"s\"t")] // quotes in the middle of arguments (no whitespace)
        [InlineData(new[] { "bad" }, "ba\"d")] // unclosed quote
        [InlineData(new[] { "" }, "\"")] // just a quote!
        public void Lex_NoRoundTrip(string[] expected, string input)
        {
            var lexed = QuoteBasedCommandLineLexer.Instance.Lex(input).ToList();
            Assert.Equal(expected, lexed);
        }

        [Fact]
        public void Lex_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => QuoteBasedCommandLineLexer.Instance.Lex(null).ToList());
        }

        [Fact]
        public void Escape_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => QuoteBasedCommandLineLexer.Instance.Escape(null));
        }
    }
}
