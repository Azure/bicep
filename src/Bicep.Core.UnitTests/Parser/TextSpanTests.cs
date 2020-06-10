using Bicep.Core.Parser;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parser
{
    [TestClass]
    public class TextSpanTests
    {
        [DataTestMethod]
        [DataRow(1, 2, "[1:3]")]
        [DataRow(0, 0, "[0:0]")]
        [DataRow(10, 20, "[10:30]")]
        public void Constructor_ShouldPreserveLocation(int position, int length, string expectedRange)
        {
            var span = new TextSpan(position, length);
            span.Position.Should().Be(position);
            span.Length.Should().Be(length);

            span.ToString().Should().Be(expectedRange);
        }

        [DataTestMethod]
        [DataRow(10, 1, 20, 2, "[10:22]")]
        [DataRow(0, 0, 0, 0, "[0:0]")]
        public void Between_ShouldCalculateCorrectRange(int firstPosition, int firstLength, int secondPosition, int secondLength, string expectedBetweenRange)
        {
            var first = new TextSpan(firstPosition, firstLength);
            var second = new TextSpan(secondPosition, secondLength);

            TextSpan.Between(first, second).ToString().Should().Be(expectedBetweenRange);
        }

        [DataTestMethod]
        [DataRow(10, 1, 20, 2, "[11:20]")]
        [DataRow(0, 0, 0, 0, "[0:0]")]
        public void BetweenNonInclusive_ShouldCalculateCorrectRange(int firstPosition, int firstLength, int secondPosition, int secondLength, string expectedBetweenRange)
        {
            var first = new TextSpan(firstPosition, firstLength);
            var second = new TextSpan(secondPosition, secondLength);

            TextSpan.BetweenNonInclusive(first, second).ToString().Should().Be(expectedBetweenRange);
        }
    }
}
