// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parsing
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
        [DataRow(70, 0, 60, 9, "[60:70]")]
        public void Between_ShouldCalculateCorrectRange(int firstPosition, int firstLength, int secondPosition, int secondLength, string expectedBetweenRange)
        {
            var first = new TextSpan(firstPosition, firstLength);
            var second = new TextSpan(secondPosition, secondLength);

            TextSpan.Between(first, second).ToString().Should().Be(expectedBetweenRange);
            TextSpan.Between(second, first).ToString().Should().Be(expectedBetweenRange);
        }

        [DataTestMethod]
        [DataRow(10, 1, 20, 2, "[11:20]")]
        [DataRow(0, 0, 0, 0, "[0:0]")]
        [DataRow(70, 0, 60, 9, "[69:70]")]
        public void BetweenNonInclusive_ShouldCalculateCorrectRange(int firstPosition, int firstLength, int secondPosition, int secondLength, string expectedBetweenRange)
        {
            var first = new TextSpan(firstPosition, firstLength);
            var second = new TextSpan(secondPosition, secondLength);

            TextSpan.BetweenExclusive(first, second).ToString().Should().Be(expectedBetweenRange);
            TextSpan.BetweenExclusive(second, first).ToString().Should().Be(expectedBetweenRange);
        }

        [DataTestMethod]
        [DataRow("[0:0]", "[0:0]", false)]
        [DataRow("[0:1]", "[1:1]", false)]
        [DataRow("[0:1]", "[0:0]", false)]
        [DataRow("[0:1]", "[0:1]", true)]
        [DataRow("[0:1]", "[0:5]", true)]
        [DataRow("[0:4]", "[4:6]", false)]
        [DataRow("[0:4]", "[3:4]", true)]
        [DataRow("[520:580]", "[1000:1100]", false)]
        [DataRow("[520:580]", "[530:540]", true)]
        [DataRow("[520:520]", "[530:540]", false)]
        public void AreOverlapping_ShouldDetermineOverlapCorrectly(string firstSpan, string secondSpan, bool expectedOverlapResult)
        {
            var first = ParseSpan(firstSpan);
            var second = ParseSpan(secondSpan);

            TextSpan.AreOverlapping(first, second).Should().Be(expectedOverlapResult);
            TextSpan.AreOverlapping(second, first).Should().Be(expectedOverlapResult);
        }

        [DataTestMethod]
        [DataRow("[0:0]", "[0:0]", true)]
        [DataRow("[10:12]", "[12:13]", true)]
        [DataRow("[12:13]", "[10:12]", false)]
        [DataRow("[100:200]", "[105:200]", false)]
        [DataRow("[100:200]", "[105:205]", false)]
        public void AreNeighbors_ShouldProduceCorrectResult(string firstSpan, string secondSpan, bool expectedResult)
        {
            var first = ParseSpan(firstSpan);
            var second = ParseSpan(secondSpan);

            TextSpan.AreNeighbors(first, second).Should().Be(expectedResult);
        }

        [DataTestMethod]
        [DataRow("[0:0]", "[0:0]", "[0:0]")]
        [DataRow("[0:2]", "[2:3]", "[0:2]")]
        [DataRow("[2:3]", "[0:2]", "[2:3]")]
        [DataRow("[14:18]", "[35:48]", "[14:35]")]
        [DataRow("[35:48]", "[14:18]", "[18:48]")]
        public void BetweenInclusiveAndExclusive_ShouldProduceCorrectSpan(string inclusiveSpan, string exclusiveSpan, string expected)
        {
            var inclusive = ParseSpan(inclusiveSpan);
            var exclusive = ParseSpan(exclusiveSpan);

            // this operation is not commutative
            TextSpan.BetweenInclusiveAndExclusive(inclusive, exclusive).ToString().Should().Be(expected);
        }

        [DataTestMethod]
        [DataRow("[0:0]")]
        [DataRow("[0:1]")]
        [DataRow("[123:134]")]
        public void GetEndPosition_ShouldReturnExpectedValue(string str)
        {
            var span = ParseSpan(str);
            span.GetEndPosition().Should().Be(span.Position + span.Length);
        }

        private static TextSpan ParseSpan(string value)
        {
            var colonIndex = value.IndexOf(':');
            var startInclusive = int.Parse(value.AsSpan(1, colonIndex - 1));
            var endExclusive = int.Parse(value.AsSpan(colonIndex + 1, value.Length - colonIndex - 2));
            var length = endExclusive - startInclusive;

            return new TextSpan(startInclusive, length);
        }
    }
}

