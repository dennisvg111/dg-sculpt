using DG.Sculpt.Cron;
using DG.Sculpt.Cron.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace DG.Sculpt.Tests.Cron
{
    public class CronValueParserTests
    {
        private readonly static CronValueParser _parser = new CronValueParser("test-field", 1, 10, 2, "ONE", "TWO");

        [Theory]
        [InlineData("1", 1)]
        [InlineData("10", 10)]
        [InlineData("11", 1)]
        [InlineData("12", 2)]
        [InlineData("TWO", 2)]
        public void Parse_Works(string value, int expected)
        {
            var result = _parser.Parse(value);
            result.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("13")]
        [InlineData("THREE")]
        public void Parse_Throws(string value)
        {
            Action action = () => _parser.Parse(value);
            action.Should().Throw<CronParsingException>();
        }
    }
}
