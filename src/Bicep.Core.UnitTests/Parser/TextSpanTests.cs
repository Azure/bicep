using System;
using Bicep.Core.Parser;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parser
{
    [TestClass]
    public class TextSpanTests
    {
        [TestMethod]
        public void NegativePosition_ShouldThrow()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action negativePosition = () => new TextSpan(-1, 0);

            negativePosition.Should().Throw<ArgumentException>().WithMessage("Position must not be negative. (Parameter 'position')");
        }

        [TestMethod]
        public void NegativeLength_ShouldThrow()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action negativeLength = () => new TextSpan(0, -1);

            negativeLength.Should().Throw<ArgumentException>().WithMessage("Length must not be negative. (Parameter 'length')");
        }

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

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("\r")]
        [DataRow("\n")]
        [DataRow("[")]
        [DataRow("[a")]
        [DataRow("[11")]
        [DataRow("[12:]")]
        [DataRow("[]")]
        [DataRow("[:]")]
        [DataRow("[1:99999999999999999999999999999999999999999999]")]
        [DataRow("[23333333333333333333333333333333333333:22]")]
        [DataRow("[123:121]")]
        public void InvalidSpanString_ShouldNotParse(string str)
        {
            TextSpan.TryParse(str, out var span).Should().BeFalse();
            span.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("[0:0]", 0, 0)]
        [DataRow("[0:1]", 0, 1)]
        [DataRow("[123:134]", 123, 11)]
        public void ValidSpanString_ShouldParse(string str, int expectedPosition, int expectedLength)
        {
            TextSpan.TryParse(str, out var span).Should().BeTrue();
            span.Should().NotBeNull();
            span!.Position.Should().Be(expectedPosition);
            span!.Length.Should().Be(expectedLength);
        }
    }
}
