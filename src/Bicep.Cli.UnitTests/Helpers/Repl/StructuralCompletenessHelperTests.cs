// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Cli.Helpers.Repl;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Cli.UnitTests.Helpers.Repl;

[TestClass]
public class StructuralCompletenessHelperTests
{
    [DataTestMethod]
    [DataRow("42", true)]
    [DataRow("'hello'", true)]
    [DataRow("true", true)]
    [DataRow("items([1, 2, 3])", true)]
    [DataRow("var a = 'complete'", true)]
    [DataRow("length(['a', 'b'])", true)]
    [DataRow(
"""
'''
Hello
World
'''
""", true)]
    [DataRow("items({", false)]
    [DataRow("var a = items({", false)]
    [DataRow("var b = length([", false)]
    [DataRow("[", false)]
    [DataRow("{", false)]
    [DataRow("(length(", false)]
    [DataRow("filter(map([1,2],", false)]
    [DataRow(
"""
filter(map([1,2],
x => x + 1),
y => y % 2 == 0
""", false)]
    [DataRow(
"""
'''
Hello
World
""", false)]
    public void IsStructurallyComplete_CompleteExpressions_ReturnsExpectedResult(string text, bool expected)
    {
        StructuralCompletenessHelper.IsStructurallyComplete(text).Should().Be(expected);
    }
}
