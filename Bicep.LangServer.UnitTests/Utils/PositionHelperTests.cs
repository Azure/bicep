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
        public void GetPosition_EmptyLineStarts_ThrowsArgumentException()
        {
            Action sut = () => PositionHelper.GetPosition(new List<int>().AsReadOnly(), 10);

            sut.Should().Throw<ArgumentException>().WithMessage("*must not be empty*");
        }

        [TestMethod]
        public void GetPosition_NegtiveOffset_ThrowsArgumentOutOfRangeException()
        {
            IReadOnlyList<int> lineStarts = new List<int> { 0, 24 }.AsReadOnly();
            Action sut = () => PositionHelper.GetPosition(lineStarts, -9);

            sut.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*must not be a negative number*");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
        public void GetPosition_ValidLineStartsAndOffset_ReturnsConvertedPosition(IReadOnlyList<int> lineStarts, int offset, Position expectedPosition)
        {
            Position position = PositionHelper.GetPosition(lineStarts, offset);

            position.Should().Be(expectedPosition);
        }

        public static IEnumerable<object[]> GetData()
        {
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 0, new Position(0, 0) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 5, new Position(0, 5) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 11, new Position(0, 11) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 12, new Position(1, 0) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 34, new Position(1, 22) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 44, new Position(1, 32) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 45, new Position(2, 0) };
            yield return new object[] { new List<int> { 0, 12, 45 }.AsReadOnly(), 99, new Position(2, 54) };
            yield return new object[] {new List<int> {0, 7, 9}.AsReadOnly(), 0, new Position(0, 0)};
        }
    }
}
