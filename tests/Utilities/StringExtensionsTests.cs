using DG.Sculpt.Utilities;
using FluentAssertions;
using Xunit;

namespace DG.Sculpt.Tests.Utilities
{
    public class StringExtensionsTests
    {
        [Fact]
        public void TrySplitOn_ContainingSplitOn_ReturnsFullAndSplits()
        {
            var s = "Hello world!";
            var splitOn = " w";

            bool result = s.TrySplitOn(splitOn, out string a, out string b);
            result.Should().BeTrue();

            a.Should().Be("Hello");
            b.Should().Be("orld!");
        }

        [Fact]
        public void TrySplitOn_NotContainingSplitOn_ReturnsFalseAndReturnsFull()
        {
            var s = "Hello world!";
            var splitOn = "-";

            bool result = s.TrySplitOn(splitOn, out string a, out string _);
            result.Should().BeFalse();

            a.Should().Be("Hello world!");
        }

        [Fact]
        public void TrySplitOn_SplitOnAtStart_ReturnsFullAndSplits()
        {
            var s = "Hello world!";
            var splitOn = "He";

            bool result = s.TrySplitOn(splitOn, out string a, out string b);
            result.Should().BeTrue();

            a.Should().Be("");
            b.Should().Be("llo world!");
        }

        [Fact]
        public void TrySplitOn_SplitOnAtEnd_ReturnsFullAndSplits()
        {
            var s = "Hello world!";
            var splitOn = "d!";

            bool result = s.TrySplitOn(splitOn, out string a, out string b);
            result.Should().BeTrue();

            a.Should().Be("Hello worl");
            b.Should().Be("");
        }
    }
}
