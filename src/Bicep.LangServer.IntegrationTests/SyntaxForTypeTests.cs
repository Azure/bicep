using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Refactor;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.IntegrationTests;

[TestClass]
public class SyntaxForTypeTests
{
    // asdfg allowed values

    // Data row args:
    //   - input: type definition for type 't'
    //   - expected: result of calling with loose strictness
    //   - expected: result of calling with medium strictness
    //   - expected: result of calling with strict strictness

    [DataTestMethod]
    [DataRow(
        "type t = 123",
        "int",
        "int",
        "123")]
    [DataRow(
        "type t = 'abc'",
        "string",
        "string",
        "'abc'")]
    [DataRow(
        "type t = true",
        "bool",
        "bool",
        "true")]
    public void LiteralTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type t = int",
        "int",
        "int",
        "int")]
    [DataRow(
        "type t = string",
        "string",
        "string",
        "string")]
    [DataRow(
        "type t = bool",
        "bool",
        "bool",
        "bool")]
    [DataRow(
        "type t = null|'abc'",
        "string",
        "'abc' | null",
        "'abc' | null")]
    public void SimpleTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type t = {a:123,b:'abc'}",
        "object",
        "{ a: int, b: string }",
        "{ a: 123, b: 'abc' }")]
    [DataRow(
        "type t = { 'my type': 'my string' }",
        "object",
        "{ 'my type': string }",
        "{ 'my type': 'my string' }")]
    [DataRow(
        "type t = { 'true': true }",
        "object",
        "{ 'true': bool }",
        "{ 'true': true }")]
    [DataRow(
        "type t = object",
        "object",
        "object",
        "object")]
    public void ObjectTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type t = 'abc' | 'def' | 'ghi'",
        "string",
        "'abc' | 'def' | 'ghi'",
        "'abc' | 'def' | 'ghi'")]
    [DataRow(
        "type t = 1 | 2 | 3 | -1",
        "int",
        "-1 | 1 | 2 | 3",
        "-1 | 1 | 2 | 3")]
    [DataRow(
        "type t = true|false",
        "bool",
        "false | true",
        "false | true")]
    public void UnionTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type t = [int, string]",
        "array",
        "[int, string]",
        "[int, string]")]
    [DataRow(
        "type t = [123, 'abc' | 'def']",
        "array",
        "[int, 'abc' | 'def']",
        "[123, 'abc' | 'def']")]
    public void TupleTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        """
                type t1 = { a: 'abc', b: 123 }
                type t = { a: t1, b: [t1, t1] }
                """,
        "object",
        // TODO: "{ a: t1, b: [t1, t1] }", asdfg
        "{ a: { a: string, b: int }, b: [{ a: string, b: int }, { a: string, b: int }] }",
        // TODO: "{ a: t1, b: [t1, t1] }"
        "{ a: { a: 'abc', b: 123 }, b: [{ a: 'abc', b: 123 }, { a: 'abc', b: 123 }] }")]
    public void NestedNamedTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [Ignore] //asdfg
    [DataRow(
        "type t = [string, t?]",
        "array",
        "[string, t?]",
        "[string, t?]")]
    public void RecursiveTypes(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        "type t = string[]",
        "array",
        "string[]",
        "string[]")]
    [DataRow(
        "type t = 'abc'[]",
        "array",
        "string[]",
        "'abc'[]")]
    [DataRow(
        "type t = ('abc'|'def')[]",
        "array",
        "('abc' | 'def')[]",
        "('abc' | 'def')[]")]
    public void TypedArrays(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromTypeDeclaration(typeDeclaration, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    [DataTestMethod]
    [DataRow(
        """
            var isWindowsOS = true
            var provisionExtensions = true
            param _artifactsLocation string
            @secure()
            param _artifactsLocationSasToken string

            resource resourceWithProperties 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' = if (isWindowsOS && provisionExtensions) {
              name: 'cse-windows/extension'
              location: 'location'
              properties: {
                publisher: 'Microsoft.Compute'
                type: 'CustomScriptExtension'
                typeHandlerVersion: '1.8'
                autoUpgradeMinorVersion: true
                settings: {
                  fileUris: [
                    uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                  ]
                  commandToExecute: 'commandToExecute'
                }
              }
            }
            """,
        "properties",
        "int",
        "int",
        "int")]
    [DataRow(
        """
            resource vm 'Microsoft.Compute/virtualMachines@2019-12-01' = { name: 'vm', location: 'eastus'
              properties: {
                diagnosticsProfile: {
                  bootDiagnostics: {
                    storageUri: reference|(storageAccount.id, '2018-02-01').primaryEndpoints.blob
                  }
                }
              }
            }

            resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = { name: 'storageaccountname' }
            """,
        "properties",
        "int",
        "int",
        "int")]
    [DataRow(
        """
            var _artifactsLocation = 'https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/azuredeploy.json'
            var _artifactsLocationSasToken = '?sas=abcd'
            var commandToExecute = 'powershell -ExecutionPolicy Unrestricted -File writeblob.ps1'

            resource r 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' {
                properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    settings: {
                        fileUris: [
                            uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                        ]
                        commandToExecute: commandToExecute
                    }
                }
            }         
            """,
        "fileUris",
        "array",
        "[string]", // the property is typed as a tuple, not string[]
        "['https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/writeblob.ps1?sas=abcd']")]
    [DataRow(
        """
            var _artifactsLocation = 'https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/azuredeploy.json'
            var _artifactsLocationSasToken = '?sas=abcd'
            var commandToExecute = 'powershell -ExecutionPolicy Unrestricted -File writeblob.ps1'

            resource r 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' {
                properties: {
                    publisher: 'Microsoft.Compute'
                    type: 'CustomScriptExtension'
                    typeHandlerVersion: '1.8'
                    autoUpgradeMinorVersion: true
                    settings: {
                        fileUris: [
                            uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
                        ]
                        commandToExecute: commandToExecute
                    }
                }
            }         
            """,
        "settings",
        "object",
        "{ commandToExecute: string, fileUris: [string] }",
        "{ commandToExecute: 'powershell -ExecutionPolicy Unrestricted -File writeblob.ps1', fileUris: ['https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/101-vm-simple-windows/writeblob.ps1?sas=abcd'] }")]
    //[DataRow(asdfg
    //    """
    //        resource r 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' {
    //            properties: {
    //                publisher: 'Microsoft.Compute'
    //                type: 'CustomScriptExtension'
    //                typeHandlerVersion: '1.8'
    //                autoUpgradeMinorVersion: true
    //                settings: {
    //                    fileUris: [
    //                        uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
    //                    ]
    //                    commandToExecute: commandToExecute
    //                }
    //            }
    //        }         
    //        """,
    //    "properties",
    //    "array",
    //    "('abc' | 'def')[]",
    //    "('abc' | 'def')[]")]
    //[DataRow(asdfg
    //    """
    //        resource r 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' {
    //            properties: {
    //                publisher: 'Microsoft.Compute'
    //                type: 'CustomScriptExtension'
    //                typeHandlerVersion: '1.8'
    //                autoUpgradeMinorVersion: true
    //                settings: {
    //                    fileUris: [
    //                        uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
    //                    ]
    //                    commandToExecute: commandToExecute
    //                }
    //            }
    //        }         
    //        """,
    //    "properties",
    //    "array",
    //    "('abc' | 'def')[]",
    //    "('abc' | 'def')[]")]
    //[DataRow(asdfg
    //    """
    //        asdfg
    //        resource r 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' {
    //            properties: {
    //                publisher: 'Microsoft.Compute'
    //                type: 'CustomScriptExtension'
    //                typeHandlerVersion: '1.8'
    //                autoUpgradeMinorVersion: true
    //                settings: {
    //                    fileUris: [
    //                        uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
    //                    ]
    //                    commandToExecute: commandToExecute
    //                }
    //            }
    //        }         
    //        """,
    //    "properties",
    //    "array",
    //    "('abc' | 'def')[]",
    //    "('abc' | 'def')[]")]
    //[DataRow(asdfg
    //    """
    //        resource r 'Microsoft.Compute/virtualMachines/extensions@2019-12-01' {
    //            properties: {
    //                publisher: 'Microsoft.Compute'
    //                type: 'CustomScriptExtension'
    //                typeHandlerVersion: '1.8'
    //                autoUpgradeMinorVersion: true
    //                settings: {
    //                    fileUris: [
    //                        uri(_artifactsLocation, 'writeblob.ps1${_artifactsLocationSasToken}')
    //                    ]
    //                    commandToExecute: commandToExecute
    //                }
    //            }
    //        }         
    //        """,
    //    "properties",
    //    "array",
    //    "('abc' | 'def')[]",
    //    "('abc' | 'def')[]")]
    public void asdfg(string resourceDeclaration, string resourcePropertyName, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        RunTestFromResourceProperty(resourceDeclaration, resourcePropertyName, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    #region Support

    // input is a type declaration statement such as "type t = int"
    private static void RunTestFromTypeDeclaration(string typeDeclaration, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        var compilationResult = CompilationHelper.Compile(typeDeclaration);
        var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
        var declarationSyntax = semanticModel.Root.TypeDeclarations[0].DeclaringSyntax;
        var declaredType = semanticModel.GetDeclaredType(semanticModel.Root.TypeDeclarations.Single(t => t.Name == "t").Value);

        RunTestHelper(declaredType, semanticModel, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    // input is a resource declaration and a property name such as "properties" that is exposed anywhere on the resource type
    private static void RunTestFromResourceProperty(string resourceDeclaration, string resourcePropertyName, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        var compilationResult = CompilationHelper.Compile(resourceDeclaration);
        var semanticModel = compilationResult.Compilation.GetEntrypointSemanticModel();
        var resourceSyntax = semanticModel.Root.ResourceDeclarations[0].DeclaringResource;

        var properties = GetAllSyntaxOfType<ObjectPropertySyntax>(resourceSyntax);
        var matchingProperty = properties.Single(p => p.Key is IdentifierSyntax id && id.NameEquals(resourcePropertyName));
        var declaredType = semanticModel.GetDeclaredType(matchingProperty);

        RunTestHelper(declaredType, semanticModel, expectedLooseSyntax, expectedMediumStrictSyntax, expectedStrictSyntax);
    }

    private static void RunTestHelper(TypeSymbol typeSymbol, SemanticModel semanticModel, string expectedLooseSyntax, string expectedMediumStrictSyntax, string expectedStrictSyntax)
    {
        PrintAllTypes(semanticModel);

        var looseSyntax = SyntaxForType.GetSyntaxStringForType(typeSymbol, SyntaxForType.Strictness.Loose);
        var mediumStrictSyntax = SyntaxForType.GetSyntaxStringForType(typeSymbol, SyntaxForType.Strictness.Medium);
        var strictSyntax = SyntaxForType.GetSyntaxStringForType(typeSymbol, SyntaxForType.Strictness.Strict);

        using (new AssertionScope())
        {
            CompilationHelper.Compile($"type t = {expectedLooseSyntax}").Diagnostics.Should().NotHaveAnyDiagnostics("expected loose syntax should be error-free");
            CompilationHelper.Compile($"type t = {expectedMediumStrictSyntax}").Diagnostics.Should().NotHaveAnyDiagnostics("expected medium strictness syntax should be error-free");
            CompilationHelper.Compile($"type t = {expectedStrictSyntax}").Diagnostics.Should().NotHaveAnyDiagnostics("expected strict syntax should be error-free");
        }

        using (new AssertionScope())
        {
            looseSyntax.Should().Be(expectedLooseSyntax);
            mediumStrictSyntax.Should().Be(expectedMediumStrictSyntax);
            strictSyntax.Should().Be(expectedStrictSyntax);
        }

        // Note: TODO: This might not be true for all cases, might have to remove this check eventually (or fix typeSymbol.Name if it makes sense)
        var typeName = typeSymbol.Name;
        if (!typeName.Contains("t1")) // doesn't currently work for recursive and named types
        {
            typeName.Should().EqualIgnoringWhitespace(expectedStrictSyntax, "the resulting strict syntax should be the same as the type's Name property");
        }
    }

    private static IEnumerable<TSyntax> GetAllSyntaxOfType<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase
        => SyntaxVisitor.GetAllSyntaxOfType<TSyntax>(syntax);

    private class SyntaxVisitor : CstVisitor
    {
        private readonly List<SyntaxBase> syntaxList = new();

        private SyntaxVisitor()
        {
        }

        public static IEnumerable<TSyntax> GetAllSyntaxOfType<TSyntax>(SyntaxBase syntax) where TSyntax : SyntaxBase
        {
            var visitor = new SyntaxVisitor();
            visitor.Visit(syntax);

            return visitor.syntaxList.OfType<TSyntax>();
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            syntaxList.Add(syntax);
            base.VisitInternal(syntax);
        }

    }

    #endregion Support


    //asdfg: remove
    private static void PrintAllTypes(SemanticModel semanticModel)
    {
        var asdfg = SyntaxCollectorVisitor.Build(semanticModel.Root.Syntax);
        foreach (var node1 in asdfg.Where(s => s.Syntax is not Token))
        {
            //asdfg
            var node = node1.Syntax;
            Trace.WriteLine($"** {node.GetDebuggerDisplay().ReplaceNewlines(" ").TruncateWithEllipses(150)}");
            Trace.WriteLine($"  ... type info: {semanticModel.GetTypeInfo(node).Name}");
            Trace.WriteLine($"  ... declared type: {semanticModel.GetDeclaredType(node)?.Name}");
        }
    }

    //asdfg: remove
    public class SyntaxCollectorVisitor : CstVisitor
    {
        public record SyntaxItem(SyntaxBase Syntax, SyntaxItem? Parent, int Depth)
        {
            public IEnumerable<SyntaxCollectorVisitor.SyntaxItem> GetAncestors()
            {
                var data = this;
                while (data.Parent is { } parent)
                {
                    yield return parent;
                    data = parent;
                }
            }
        }

        private readonly IList<SyntaxItem> syntaxList = new List<SyntaxItem>();
        private SyntaxItem? parent = null;
        private int depth = 0;

        private SyntaxCollectorVisitor()
        {
        }

        public static ImmutableArray<SyntaxItem> Build(SyntaxBase syntax)
        {
            var visitor = new SyntaxCollectorVisitor();
            visitor.Visit(syntax);

            return [..visitor.syntaxList];
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            var syntaxItem = new SyntaxItem(Syntax: syntax, Parent: parent, Depth: depth);
            syntaxList.Add(syntaxItem);

            var prevParent = parent;
            parent = syntaxItem;
            depth++;
            base.VisitInternal(syntax);
            depth--;
            parent = prevParent;
        }
    }
}

