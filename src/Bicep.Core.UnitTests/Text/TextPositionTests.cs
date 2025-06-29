// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Text;

[TestClass]
public class TextPositionTests
{
    [DataTestMethod]
    [DataRow(-1, 0)]
    [DataRow(0, -1)]
    public void TextPosition_NegativeLineOrCharacter_Throws(int line, int character)
    {
        FluentActions.Invoking(() => new TextPosition(line, character)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [DataTestMethod]
    [DataRow(10, 0)]
    [DataRow(0, 10)]
    public void TextPosition_NonNegativeLineOrCharacter_DoesNotThrow(int line, int character)
    {
        var position = new TextPosition(line, character);

        position.Line.Should().Be(line);
        position.Character.Should().Be(character);
    }
}
