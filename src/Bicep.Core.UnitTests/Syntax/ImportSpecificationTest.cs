// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Syntax;

[TestClass]
public class ImportSpecificationTests
{
    [TestMethod]
    [DataRow("br/public:az@1.0.0", true, "az")]
    [DataRow("br:mcr.microsoft.com/bicep/providers/az@1.0.0", true, "az")]
    [DataRow("kubernetes@1.0.0", true, "kubernetes")]
    [DataRow("sys@1.0.0", true, "sys")]
    [DataRow("br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta", true, "az")] // valid prerelease version
    [DataRow("br:mcr.microsoft.com/bicep/providers/az@1.0.0+build.123", true, "az")] // valid build metadata
    [DataRow("br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta+build.123", true, "az")] // valid prerelease version and build metadata
    [DataRow("br:mcr.microsoft.com/bicep/providers/kubernetes@1.0.0", true, "kubernetes")] // other provider
    // Negative cases
    [DataRow("az@1.0.0", false)] // 'az@1.0.0' is not a valid provider declaration statement
    [DataRow("br/public:az", false)] // is not a valid provider declaration statement, it lacks a version
    [DataRow("br:mcr.microsoft.com/bicep/providers/az@", false)] // is not a valid provider declaration statement, partially constructed
    [DataRow("br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta+build.123+extra", false)] // invalid syntax, multiple build metadata sections
    [DataRow("br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta+build.123+extra.456", false)] // invalid syntax, build metadata section contains a dot
    public void TestCreateFromStringSyntax(string input, bool isValidDeclaration, string expectedName = LanguageConstants.ErrorName)
    {
        // Arrange
        var syntax = SyntaxFactory.CreateStringLiteralWithTextSpan(input);
        // Act
        var got = ImportSpecification.From(syntax);
        // Assert
        if (isValidDeclaration)
        {
            got.Name.Should().Be(expectedName);
        }
        got.IsValid.Should().Be(isValidDeclaration);
    }
}
