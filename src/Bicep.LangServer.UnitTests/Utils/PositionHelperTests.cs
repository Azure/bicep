using System;
using System.Collections.Generic;
using Bicep.LanguageServer.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.UnitTests.Utils
{
    [TestClass]
    public class PositionHelperTests
    {
        [TestMethod]
        public void GetLineStarts_EmptyContents_ReturnsZero()
        {
            IReadOnlyList<int> lineStarts = PositionHelper.GetLineStarts(string.Empty);

            lineStarts.Should().HaveCount(1);
            lineStarts[0].Should().Be(0);
        }

        [TestMethod]
        public void GetLineStarts_SingleLine_ReturnsZero()
        {
            IReadOnlyList<int> lineStarts = PositionHelper.GetLineStarts("The quick brown fox jumps over the lazy dog");

            lineStarts.Should().HaveCount(1);
            lineStarts[0].Should().Be(0);
        }

        [DataTestMethod]
        [DataRow("\r", new[] { 0, 1 })]
        [DataRow("\r\r\r\r", new[] { 0, 1, 2, 3, 4 })]
        [DataRow("foobar\r", new[] { 0, 7 })]
        [DataRow("foo\rbar\r\rbaz", new[] { 0, 4, 8, 9 })]
        [DataRow("foo\rbar\rbaz\r", new[] { 0, 4, 8, 12 })]
        [DataRow("\rfoo\rbar\rbaz", new[] { 0, 1, 5, 9 })]
        [DataRow("\rfoo\rbar\rbaz\rfoobar\r", new[] { 0, 1, 5, 9, 13, 20 })]
        public void GetLineStarts_CarriageReturns_ReturnsCorrectLineStarts(string contents, int[] expectedLineStarts)
        {
            IReadOnlyList<int> lineStarts = PositionHelper.GetLineStarts(contents);

            lineStarts.Should().HaveCount(expectedLineStarts.Length);
            lineStarts.Should().BeEquivalentTo(expectedLineStarts);
        }

        [DataTestMethod]
        [DataRow("\n", new[] { 0, 1 })]
        [DataRow("\n\n\n\n", new[] { 0, 1, 2, 3, 4 })]
        [DataRow("foobar\n", new[] { 0, 7 })]
        [DataRow("foo\nbar\n\nbaz", new[] { 0, 4, 8, 9 })]
        [DataRow("foo\nbar\nbaz\n", new[] { 0, 4, 8, 12 })]
        [DataRow("\nfoo\nbar\nbaz", new[] { 0, 1, 5, 9 })]
        [DataRow("\nfoo\nbar\nbaz\nfoobar\n", new[] { 0, 1, 5, 9, 13, 20 })]
        public void GetLineStarts_LineFeeds_ReturnsCorrectLineStarts(string contents, int[] expectedLineStarts)
        {
            IReadOnlyList<int> lineStarts = PositionHelper.GetLineStarts(contents);

            lineStarts.Should().HaveCount(expectedLineStarts.Length);
            lineStarts.Should().BeEquivalentTo(expectedLineStarts);
        }

        [DataTestMethod]
        [DataRow("\r\n", new[] { 0, 2 })]
        [DataRow("\r\n\r\n\r\n\r\n", new[] { 0, 2, 4, 6, 8 })]
        [DataRow("foobar\r\n", new[] { 0, 8 })]
        [DataRow("foo\r\nbar\r\n\r\nbaz", new[] { 0, 5, 10, 12 })]
        [DataRow("foo\r\nbar\r\nbaz\r\n", new[] { 0, 5, 10, 15 })]
        [DataRow("\r\nfoo\r\nbar\r\nbaz", new[] { 0, 2, 7, 12 })]
        [DataRow("\r\nfoo\r\nbar\r\nbaz\r\nfoobar\r\n", new[] { 0, 2, 7, 12, 17, 25 })]
        public void GetLineStarts_CRLFs_ReturnsCorrectLineStarts(string contents, int[] expectedLineStarts)
        {
            IReadOnlyList<int> lineStarts = PositionHelper.GetLineStarts(contents);

            lineStarts.Should().HaveCount(expectedLineStarts.Length);
            lineStarts.Should().BeEquivalentTo(expectedLineStarts);
        }

        [DataTestMethod]
        [DataRow("\n\r", new[] { 0, 1, 2 })]
        [DataRow("\n\r\n", new[] { 0, 1, 3 })]
        [DataRow("\n\r\r", new[] { 0, 1, 2, 3 })]
        [DataRow("\n\r\r\n", new[] { 0, 1, 2, 4 })]
        [DataRow("\n\r\n\n", new[] { 0, 1, 3, 4 })]
        [DataRow("\r\n\n\r", new[] { 0, 2, 3, 4 })]
        [DataRow("foo\nbar\rbaz\r\nfoobar", new[] { 0, 4, 8, 13 })]
        [DataRow("\nfoo\r\nbar\r\nbaz\r", new[] { 0, 1, 6, 11, 15 })]
        [DataRow("\r\nfoo\rbar\nbaz\r\nfoobar\n", new[] { 0, 2, 6, 10, 15, 22 })]
        public void GetLineStarts_MixedEndOfLines_ReturnsCorrectLineStarts(string contents, int[] expectedLineStarts)
        {
            IReadOnlyList<int> lineStarts = PositionHelper.GetLineStarts(contents);

            lineStarts.Should().HaveCount(expectedLineStarts.Length);
            lineStarts.Should().BeEquivalentTo(expectedLineStarts);
        }

        [TestMethod]
        public void GetPosition_EmptyLineStarts_ThrowsArgumentException()
        {
            Action sut = () => PositionHelper.GetPosition(new List<int>().AsReadOnly(), 10);

            sut.Should().Throw<ArgumentException>().WithMessage("*must not be empty*");
        }

        [TestMethod]
        public void GetPosition_FirstElementOfLineStartsIsNotZero_ThrowsArgumentException()
        {
            Action sut = () => PositionHelper.GetPosition(new List<int> { 42 }, 10);

            sut.Should().Throw<ArgumentException>().WithMessage("*must be 0, but got 42*");
        }

        [TestMethod]
        public void GetPosition_NegtiveOffset_ThrowsArgumentOutOfRangeException()
        {
            IReadOnlyList<int> lineStarts = new List<int> { 0, 24 }.AsReadOnly();
            Action sut = () => PositionHelper.GetPosition(lineStarts, -9);

            sut.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*must not be a negative number*");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetTestDataForGetPosition), DynamicDataSourceType.Method)]
        public void GetPosition_ValidLineStartsAndOffset_ReturnsConvertedPosition(IReadOnlyList<int> lineStarts, int offset, Position expectedPosition)
        {
            Position position = PositionHelper.GetPosition(lineStarts, offset);

            position.Should().Be(expectedPosition);
        }

        public static IEnumerable<object[]> GetTestDataForGetPosition()
        {
            yield return new object[] { new List<int> { 0, 7, 9 }.AsReadOnly(), 0, new Position(0, 0) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 0, new Position(0, 0) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 5, new Position(0, 5) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 11, new Position(0, 11) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 12, new Position(1, 0) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 34, new Position(1, 22) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 44, new Position(1, 32) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 45, new Position(2, 0) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 99, new Position(2, 54) };
        }
    }
}
