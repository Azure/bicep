// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Reflection;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics
{
    [TestClass]
    public class DiagnosticBuilderTests
    {
        [TestMethod]
        public void DiagnosticBuilder_CodesAreUnique()
        {
            var diagnosticMethods = typeof(DiagnosticBuilder.DiagnosticBuilderInternal)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => typeof(Diagnostic).IsAssignableFrom(m.ReturnType));

            // verify the above Linq is actually working
            diagnosticMethods.Should().HaveCountGreaterThan(40);

            var builder = DiagnosticBuilder.ForPosition(new TextSpan(0, 10));

            var definedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var diagnosticMethod in diagnosticMethods)
            {
                var mockParams = diagnosticMethod.GetParameters().Select(CreateMockParameter);

                var diagnostic = diagnosticMethod.Invoke(builder, mockParams.ToArray()) as Diagnostic;

                // verify that the Code is unique
                definedCodes.Should().NotContain(diagnostic!.Code, $"Method {diagnosticMethod.Name} should be assigned a unique error code.");
                definedCodes.Add(diagnostic!.Code);
            }
        }

        [TestMethod]
        public void UnknownPropertyWithSuggestion_ProducesPreferredPropertyReplacementCodeFix()
        {
            var builder = DiagnosticBuilder.ForPosition(new TextSpan(0, 10));

            var diagnostic = builder.UnknownPropertyWithSuggestion(false, new PrimitiveType("testType", TypeSymbolValidationFlags.Default), "networkACLs", "networkAcls");
            diagnostic.Fixes.Should().NotBeNull();
            diagnostic.Fixes.Should().HaveCount(1);

            var fix = diagnostic.Fixes.First();
            fix.IsPreferred.Should().BeTrue();
            fix.Replacements.Should().NotBeNull();
            fix.Replacements.Should().HaveCount(1);

            var replacement = fix.Replacements.First();
            replacement.Span.Should().Be(diagnostic.Span);
            replacement.Text.Should().Be("networkAcls");
        }

        [TestMethod]
        public void SymbolicNameDoesNotExistWithSuggestion_ProducesPreferredNameReplacementCodeFix()
        {
            var builder = DiagnosticBuilder.ForPosition(new TextSpan(0, 10));

            var diagnostic = builder.SymbolicNameDoesNotExistWithSuggestion("hellO", "hello");
            diagnostic.Fixes.Should().NotBeNull();
            diagnostic.Fixes.Should().HaveCount(1);

            var fix = diagnostic.Fixes.First();
            fix.IsPreferred.Should().BeTrue();
            fix.Replacements.Should().NotBeNull();
            fix.Replacements.Should().HaveCount(1);

            var replacement = fix.Replacements.First();
            replacement.Span.Should().Be(diagnostic.Span);
            replacement.Text.Should().Be("hello");
        }

        private static object CreateMockParameter(ParameterInfo parameter, int index)
        {
            if (parameter.ParameterType == typeof(TypeSymbol))
            {
                return new PrimitiveType($"<type_{index}>", TypeSymbolValidationFlags.Default);
            }

            if (parameter.ParameterType == typeof(IList<TypeSymbol>))
            {
                return new List<TypeSymbol> { new PrimitiveType($"<list_type_{index}>", TypeSymbolValidationFlags.Default) };
            }

            if (parameter.ParameterType == typeof(IEnumerable<string>) ||
                parameter.ParameterType == typeof(IList<string>) ||
                parameter.ParameterType == typeof(ICollection<string>))
            {
                return new List<string> { $"<value_{index}" };
            }

            if (parameter.ParameterType == typeof(string[]))
            {
                return new[] { $"<value_{index}" };
            }

            if (parameter.ParameterType == typeof(IDiagnosticLookup))
            {
                return new DiagnosticTree();
            }

            if (parameter.ParameterType == typeof(ImmutableArray<string>))
            {
                return new[] { $"<value_{index}" }.ToImmutableArray();
            }

            if (parameter.ParameterType == typeof(Uri))
            {
                return new Uri("file:///path/to/main.bicep");
            }

            if (parameter.ParameterType == typeof(Symbol))
            {
                // just using this one as it's easy to construct
                return ErrorType.Create([]);
            }

            if (parameter.ParameterType == typeof(int) || parameter.ParameterType == typeof(int?))
            {
                return 0;
            }

            if (parameter.ParameterType == typeof(long) || parameter.ParameterType == typeof(long?))
            {
                return 0L;
            }

            if (parameter.ParameterType == typeof(bool) || parameter.ParameterType == typeof(bool?))
            {
                return false;
            }

            if (parameter.ParameterType == typeof(SymbolKind))
            {
                return SymbolKind.Variable;
            }

            if (parameter.ParameterType == typeof(ResourceTypeReference))
            {
                return ResourceTypeReference.Parse("Mock.ErrorParam/mockResources@2020-01-01");
            }

            if (parameter.ParameterType == typeof(ResourceScope))
            {
                return ResourceScope.ResourceGroup;
            }

            if (parameter.ParameterType == typeof(string) || parameter.ParameterType == typeof(IEnumerable<char>))
            {
                return $"<param_{index}>";
            }

            if (parameter.ParameterType == typeof(ObjectSyntax))
            {
                return TestSyntaxFactory.CreateObject(Array.Empty<ObjectPropertySyntax>());
            }

            if (parameter.ParameterType == typeof(SyntaxBase))
            {
                return TestSyntaxFactory.CreateVariableAccess("identifier");
            }

            if (parameter.ParameterType == typeof(AccessExpressionSyntax))
            {
                return TestSyntaxFactory.CreatePropertyAccess(TestSyntaxFactory.CreateVariableAccess("identifier"), "propertyName");
            }

            if (parameter.ParameterType == typeof(ExtensionDeclarationSyntax))
            {
                return new ExtensionDeclarationSyntax(
                    [],
                    SyntaxFactory.ImportKeywordToken,
                    SyntaxFactory.CreateStringLiteralWithTextSpan("kubernetes@1.0.0"),
                    withClause: SyntaxFactory.EmptySkippedTrivia,
                    asClause: SyntaxFactory.EmptySkippedTrivia);
            }

            if (parameter.ParameterType == typeof(SpreadExpressionSyntax))
            {
                return new SpreadExpressionSyntax(
                    SyntaxFactory.EllipsisToken,
                    TestSyntaxFactory.CreateVariableAccess("identifier"));
            }

            if (parameter.ParameterType == typeof(ParameterizedTypeInstantiationSyntaxBase))
            {
                return new ParameterizedTypeInstantiationSyntax(
                    TestSyntaxFactory.CreateIdentifier("foo"),
                    SyntaxFactory.CreateToken(TokenType.LeftChevron),
                    TestSyntaxFactory.CreateString("RP.Namespace/widgets@v1").AsEnumerable(),
                    SyntaxFactory.CreateToken(TokenType.RightChevron));
            }

            if (parameter.ParameterType == typeof(ExportMetadataKind))
            {
                return ExportMetadataKind.Error;
            }

            if (parameter.ParameterType == typeof(BicepSourceFileKind))
            {
                return BicepSourceFileKind.BicepFile;
            }

            if (parameter.ParameterType == typeof(ArtifactType))
            {
                return ArtifactType.Module;
            }

            if (parameter.ParameterType == typeof(IOUri) || parameter.ParameterType == typeof(IOUri?))
            {
                return new IOUri("file", "", "/foo");
            }

            throw new AssertFailedException($"Unable to generate mock parameter value of type '{parameter.ParameterType}' for the diagnostic builder method.");
        }

        private static void ExpectDiagnosticWithFixedText(string text, string expectedText)
        {
            var result = CompilationHelper.Compile(text);
            result.Diagnostics.Should().HaveCount(1);

            var diagnostic = result.Diagnostics.Single();
            diagnostic.Code.Should().Be("BCP035");
            diagnostic.Fixes.Should().HaveCount(1);

            var fix = diagnostic.Fixes.Single();
            fix.Replacements.Should().HaveCount(1);

            var replacement = fix.Replacements.Single();

            var actualText = text.Remove(replacement.Span.Position, replacement.Span.Length);
            actualText = actualText.Insert(replacement.Span.Position, replacement.Text);

            // Normalize line endings
            expectedText = expectedText.ReplaceLineEndings();
            actualText = actualText.ReplaceLineEndings();

            actualText.Should().Be(expectedText);
        }

        [DataRow(@"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                 }",
            @"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                   name:
                 }"
        )]
        [DataRow(@"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {

                 }",
            @"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                   name:
                 }"
        )]
        // There is leading whitespace in this one
        [DataRow(@"
                resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                  " + @"
                }",
           @"
                resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                  name:
                }"
       )]
        [DataRow(@"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                   location: 'global'
                 }",
            @"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                   location: 'global'
                   name:
                 }"
        )]
        [DataRow(@"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                               location: 'global'
                 }",
            @"
                 resource vnet 'Microsoft.Network/virtualNetworks@2018-10-01' = {
                               location: 'global'
                               name:
                 }"
        )]
        [DataRow(@"
                 resource appService 'Microsoft.Web/serverFarms@2020-06-01' = {
                       sku: {

                         name: 'D1'

                       }
                       // comment
                 }",
            @"
                 resource appService 'Microsoft.Web/serverFarms@2020-06-01' = {
                       sku: {

                         name: 'D1'

                       }
                       
                       location:
                       name:// comment
                 }"
        )]
        [DataRow(@"
                 resource appService 'Microsoft.Web/serverFarms@2020-06-01' = {
                       sku: {}
                 }",
            @"
                 resource appService 'Microsoft.Web/serverFarms@2020-06-01' = {
                       sku: {}
                       location:
                       name:
                 }"
        )]
        [DataTestMethod]
        public void MissingTypePropertiesHasFix(string text, string expectedFix)
        {
            ExpectDiagnosticWithFixedText(text, expectedFix);
        }

        private class PrimitiveType : TypeSymbol
        {
            public PrimitiveType(string name, TypeSymbolValidationFlags validationFlags) : base(name)
            {
                ValidationFlags = validationFlags;
            }

            public override TypeKind TypeKind => TypeKind.Primitive;

            public override TypeSymbolValidationFlags ValidationFlags { get; }
        }
    }
}
