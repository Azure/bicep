// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using DiffMatchPatch;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem.Providers;

[TestClass]
public class ProviderSpecificationFactoryTests
{
    public enum SyntaxType { Identifier, InterpolableString, Other };

    [TestMethod]
    [DataRow(SyntaxType.InterpolableString, "br/public:az@1.0.0", true, "az")]
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/az@1.0.0", true, "az")]
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta", true, "az")] // valid prerelease version
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/az@1.0.0+build.123", true, "az")] // valid build metadata
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta+build.123", true, "az")] // valid prerelease version and build metadata
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/kubernetes@1.0.0", true, "kubernetes")] // other provider
    [DataRow(SyntaxType.Identifier, "sys", true, "sys")] // no version
    // The test cases below are instances of the legacy declaration syntax which is allowed until DynamicTypeLoading is GA
    [DataRow(SyntaxType.InterpolableString, "kubernetes@1.0.0", true, "kubernetes")]
    [DataRow(SyntaxType.InterpolableString, "sys@1.0.0", true, "sys")]
    [DataRow(SyntaxType.InterpolableString, "az@1.0.0", true, "az")]
    [DataRow(SyntaxType.InterpolableString, "microsoftGraph@1.0.0", true, "microsoftGraph")]
    // Negative cases
    [DataRow(SyntaxType.InterpolableString, "br/public:az", false)] // is not a valid provider declaration statement, it lacks a version
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/az@", false)] // is not a valid provider declaration statement, partially constructed
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta+build.123+extra", false)] // invalid syntax, multiple build metadata sections
    [DataRow(SyntaxType.InterpolableString, "br:mcr.microsoft.com/bicep/providers/az@1.0.0-beta+build.123+extra.456", false)] // invalid syntax, build metadata section contains a dot
    public void TestCreateFromStringSyntax(SyntaxType type, string input, bool isValidDeclaration, string expectedName = LanguageConstants.ErrorName)
    {
        // Arrange
        SyntaxBase? syntax = type switch
        {
            SyntaxType.InterpolableString => SyntaxFactory.CreateStringLiteralWithTextSpan(input),
            SyntaxType.Identifier => SyntaxFactory.CreateIdentifier(input),
            _ => default
        };
        // Act
        var got = ProviderSpecificationFactory.CreateProviderSpecification(syntax ?? throw new UnreachableException("test expects a valid syntax type"));
        // Assert
        if (isValidDeclaration)
        {
            got.NamespaceIdentifier.Should().Be(expectedName);
        }
        got.IsValid.Should().Be(isValidDeclaration);
    }
}
