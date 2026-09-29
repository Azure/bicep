// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Compilation;
using Bicep.LanguageServer.Features.Custom.Visualization;
using Bicep.LanguageServer.Utils;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using CompilationHelper = Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class VisualResourceEditingServiceTests
{
    private static readonly ImmutableArray<ResourceTypeComponents> CatalogFixture =
    [
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2020-01-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/alpha", "2021-01-01-preview", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/beta", "2020-06-01", TypeSymbolValidationFlags.Default),
        TestTypeHelper.CreateCustomResourceType("Test.Rp/gamma", "2019-01-01", TypeSymbolValidationFlags.Default),
    ];

    #region PrepareResource

    [DataTestMethod]
    [DataRow("2020-01-01")]
    [DataRow("2021-01-01-preview")]
    public void PrepareResourceCreation_UsesTheExplicitlySelectedVersion(string apiVersion)
    {
        var (compiler, result) = CompileWithResourceTypes(CatalogFixture, string.Empty);
        var context = new CompilationContext(result.Compilation);
        var request = new PrepareVisualResourceCreationParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "selected-version",
            new VisualResourceTypeIdentifier("Test.Rp/alpha", apiVersion));

        var response = new VisualResourceEditingService().PrepareResourceCreation(compiler, context, request);

        ApplyEdit(string.Empty, context.LineStarts, response.Edit).Should().Contain($"'Test.Rp/alpha@{apiVersion}'");
    }

    [TestMethod]
    public void PrepareResourceCreation_GeneratesValidTopLevelDeclarationAndVersionedEdit()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceEditingService();

        var request = new PrepareVisualResourceCreationParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 7 },
            "operation-1",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.PrepareResourceCreation(compiler, context, request);

        response.OperationId.Should().Be("operation-1");
        response.ExpectedNodeId.Should().Be("basicTest");
        response.UnresolvedRequiredProperties.Should().BeEmpty();

        var textDocumentEdit = response.Edit.DocumentChanges.Should().ContainSingle().Subject.TextDocumentEdit;
        textDocumentEdit.Should().NotBeNull();
        textDocumentEdit!.TextDocument.Uri.Should().Be(request.TextDocument.Uri);
        textDocumentEdit.TextDocument.Version.Should().Be(request.TextDocument.Version);

        var updatedContent = ApplyEdit(string.Empty, context.LineStarts, response.Edit);
        updatedContent.ReplaceLineEndings("\n").Should().Be("""
            resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
              name: 'basicTest'
            }
            """);
        var (_, updatedResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, updatedContent);
        updatedResult.Should().NotHaveAnyDiagnostics();
    }

    [TestMethod]
    public void PrepareResourceCreation_SymbolicNameCollision_GeneratesUniqueSuffixedName()
    {
        var content = """
            resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
              name: 'existing'
            }
            """;

        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        compilationResult.Should().NotHaveAnyDiagnostics();
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceEditingService();

        var request = new PrepareVisualResourceCreationParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-2",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.PrepareResourceCreation(compiler, context, request);

        response.ExpectedNodeId.Should().Be("basicTest1");

        var updatedContent = ApplyEdit(content, context.LineStarts, response.Edit);
        updatedContent.ReplaceLineEndings("\n").Should().Contain(
            "resource basicTest1 'Test.Rp/basicTests@2020-01-01' = {\n  name: 'basicTest1'\n}");
    }

    [TestMethod]
    public void PrepareResourceCreation_WithExistingResources_InsertsAfterLastResourceAndBeforeOutput()
    {
        var content = """
            resource first 'Test.Rp/basicTests@2020-01-01' = {
              name: 'first'
            }

            resource second 'Test.Rp/basicTests@2020-01-01' = {
              name: 'second'
            }

            output result string = second.name
            """;

        var updatedContent = PrepareAndApply(content);

        updatedContent.IndexOf("resource second", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal));
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
        updatedContent.ReplaceLineEndings("\n").Should().Contain("}\n\nresource basicTest");
    }

    [TestMethod]
    public void PrepareResourceCreation_WithoutExistingResource_InsertsAfterParametersAndVariablesAndBeforeOutputs()
    {
        var content = """
            param prefix string
            var resourceName = '${prefix}-resource'

            output result string = resourceName
            """;

        var updatedContent = PrepareAndApply(content);

        updatedContent.IndexOf("var resourceName", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal));
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
    }

    [TestMethod]
    public void PrepareResourceCreation_WithOnlyOutputs_InsertsBeforeFirstOutput()
    {
        var content = "output result string = 'value'";

        var updatedContent = PrepareAndApply(content);

        updatedContent.Should().StartWith("resource basicTest");
        updatedContent.IndexOf("resource basicTest", StringComparison.Ordinal)
            .Should().BeLessThan(updatedContent.IndexOf("output result", StringComparison.Ordinal));
    }

    [TestMethod]
    public void PrepareResourceCreation_ReadWriteType_ReportsUnresolvedRequiredProperties()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceEditingService();

        var request = new PrepareVisualResourceCreationParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-3",
            new VisualResourceTypeIdentifier("Test.Rp/readWriteTests", "2020-01-01"));

        var response = service.PrepareResourceCreation(compiler, context, request);

        response.UnresolvedRequiredProperties.Should().Equal("properties");
        ApplyEdit(string.Empty, context.LineStarts, response.Edit).ReplaceLineEndings("\n").Should().Be("""
            resource readWriteTest 'Test.Rp/readWriteTests@2020-01-01' = {
              name: 'readWriteTest'
              properties: {
                required:
              }
            }
            """);
    }

    [TestMethod]
    public void PrepareResourceCreation_UsesConfiguredFormattingForIncompleteValues()
    {
        var bicepConfig = """
            {
              "formatting": {
                "indentKind": "Tab",
                "newlineKind": "CRLF"
              }
            }
            """;
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty, bicepConfig);
        var context = new CompilationContext(compilationResult.Compilation);
        var request = CreateRequest("Test.Rp/readWriteTests", "2020-01-01");

        var response = new VisualResourceEditingService().PrepareResourceCreation(compiler, context, request);

        ApplyEdit(string.Empty, context.LineStarts, response.Edit).Should().Be(
            "resource readWriteTest 'Test.Rp/readWriteTests@2020-01-01' = {\r\n" +
            "\tname: 'readWriteTest'\r\n" +
            "\tproperties: {\r\n" +
            "\t\trequired:\r\n" +
            "\t}\r\n" +
            "}");
    }

    [TestMethod]
    public void PrepareResourceCreation_DiscriminatedType_ReportsDiscriminatorKeyAsUnresolved()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceEditingService();

        var request = new PrepareVisualResourceCreationParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-4",
            new VisualResourceTypeIdentifier("Test.Rp/discriminatorTests", "2020-01-01"));

        var response = service.PrepareResourceCreation(compiler, context, request);

        response.UnresolvedRequiredProperties.Should().Equal("kind");
        ApplyEdit(string.Empty, context.LineStarts, response.Edit).ReplaceLineEndings("\n").Should().Be("""
            resource discriminatorTest 'Test.Rp/discriminatorTests@2020-01-01' = {
              kind:
            }
            """);
    }

    [TestMethod]
    public void PrepareResourceCreation_VersionNotDeployableAtTheDocumentScope_Throws()
    {
        var fixture = ImmutableArray.Create(
            CreateScopedType("Scope.Rp/widgets", ResourceScope.ResourceGroup, "2024-01-01"),
            CreateScopedType("Scope.Rp/widgets", ResourceScope.Subscription, "2020-01-01"));
        var (compiler, result) = CompileWithResourceTypes(fixture, string.Empty);
        var context = new CompilationContext(result.Compilation);
        var service = new VisualResourceEditingService();

        CreateRequest("Scope.Rp/widgets", "2024-01-01").Invoking(request => service.PrepareResourceCreation(compiler, context, request))
            .Should().NotThrow();
        CreateRequest("Scope.Rp/widgets", "2020-01-01").Invoking(request => service.PrepareResourceCreation(compiler, context, request))
            .Should().Throw<VisualResourceCreationException>()
            .WithMessage("Resource type \"Scope.Rp/widgets@2020-01-01\" cannot be deployed at the \"resourceGroup\" scope.");
    }

    [TestMethod]
    public void PrepareResourceCreation_UnknownResourceType_ThrowsVisualResourceCreationException()
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, string.Empty);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceEditingService();

        var request = new PrepareVisualResourceCreationParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation-5",
            new VisualResourceTypeIdentifier("Test.Rp/doesNotExist", "2020-01-01"));

        Action act = () => service.PrepareResourceCreation(compiler, context, request);

        act.Should().Throw<VisualResourceCreationException>()
            .WithMessage("Resource type \"Test.Rp/doesNotExist@2020-01-01\" was not found.");
    }

    #endregion

    #region PrepareResourceReplays

    private const string ExistingResource = """
        param location string

        resource first 'Test.Rp/basicTests@2020-01-01' = {
          name: 'first'
        }

        output result string = first.name
        """;

    [TestMethod]
    public void PrepareResourceReplays_Undo_RemovesTheCreationDespiteUnrelatedEdits()
    {
        var (created, nodeId, insertedText) = CreateResource(ExistingResource);
        var edited = "\n" + created.Replace("output result", "\noutput result", StringComparison.Ordinal);

        var undone = Replay(edited, nodeId, VisualResourceReplayDirection.Undo, insertedText);

        undone.Should().Be("\n" + ExistingResource.Replace("output result", "\noutput result", StringComparison.Ordinal));
    }

    [TestMethod]
    public void PrepareResourceReplays_Undo_KeepsBlankLinesAddedNextToTheCreation()
    {
        var (created, nodeId, insertedText) = CreateResource(ExistingResource);
        var edited = created.Replace("}\n\nresource basicTest", "}\n\n\nresource basicTest", StringComparison.Ordinal);

        var undone = Replay(edited, nodeId, VisualResourceReplayDirection.Undo, insertedText);

        undone.Should().Be(ExistingResource.Replace("name: 'first'\n}", "name: 'first'\n}\n", StringComparison.Ordinal));
    }

    [TestMethod]
    public void PrepareResourceReplays_Undo_RemovesAnIncompleteCreation()
    {
        var (created, nodeId, insertedText) = CreateResource(ExistingResource, "Test.Rp/readWriteTests");

        Replay(created, nodeId, VisualResourceReplayDirection.Undo, insertedText).Should().Be(ExistingResource);
    }

    [TestMethod]
    public void PrepareResourceReplays_Undo_IsUnavailableOnceTheDeclarationIsEdited()
    {
        var (created, nodeId, insertedText) = CreateResource(ExistingResource);
        var edited = created.Replace("name: 'basicTest'", "name: 'renamed'", StringComparison.Ordinal);

        Replay(edited, nodeId, VisualResourceReplayDirection.Undo, insertedText).Should().BeNull();
    }

    [TestMethod]
    public void PrepareResourceReplays_Undo_IsUnavailableWhileTheDeclarationIsReferenced()
    {
        var (created, nodeId, insertedText) = CreateResource(ExistingResource);
        var edited = created + "\noutput id string = basicTest.id\n";

        Replay(edited, nodeId, VisualResourceReplayDirection.Undo, insertedText).Should().BeNull();
    }

    [TestMethod]
    public void PrepareResourceReplays_Undo_IsUnavailableOnceTheDeclarationIsRemoved()
    {
        var (_, nodeId, insertedText) = CreateResource(ExistingResource);

        Replay(ExistingResource, nodeId, VisualResourceReplayDirection.Undo, insertedText).Should().BeNull();
    }

    [TestMethod]
    public void PrepareResourceReplays_Redo_ReinsertsTheCreationAfterTheLastResource()
    {
        var (created, nodeId, insertedText) = CreateResource(ExistingResource);
        var edited = "\n" + ExistingResource;

        Replay(edited, nodeId, VisualResourceReplayDirection.Redo, insertedText).Should().Be("\n" + created);
    }

    [TestMethod]
    public void PrepareResourceReplays_Redo_IsUnavailableOnceTheSymbolicNameIsTaken()
    {
        var (_, nodeId, insertedText) = CreateResource(ExistingResource);
        var edited = ExistingResource + "\nvar BasicTest = 'taken'\n";

        Replay(edited, nodeId, VisualResourceReplayDirection.Redo, insertedText).Should().BeNull();
    }

    [TestMethod]
    public void PrepareResourceReplays_ReturnsOneResultPerCreationInRequestOrder()
    {
        var (created, nodeId, insertedText) = CreateResource(ExistingResource);
        var (_, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, created);
        var context = new CompilationContext(compilationResult.Compilation);

        var result = new VisualResourceEditingService().PrepareResourceReplays(context, new(
            new TextDocumentIdentifier(DocumentUri.From("main.bicep")),
            [
                new("undo-op", nodeId, VisualResourceReplayDirection.Undo, insertedText),
                new("redo-op", nodeId, VisualResourceReplayDirection.Redo, insertedText),
                new("unknown-op", nodeId, "sideways", insertedText),
            ]));

        result.Replays.Select(replay => replay.OperationId).Should().Equal("undo-op", "redo-op", "unknown-op");
        result.Replays.Select(replay => replay.Edit is not null).Should().Equal(true, false, false);
    }

    /// <summary>Creates a resource the way the designer does and returns the new content, node id, and inserted text.</summary>
    private static (string Content, string NodeId, string InsertedText) CreateResource(string content, string fullyQualifiedType = "Test.Rp/basicTests")
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        var context = new CompilationContext(compilationResult.Compilation);
        var response = new VisualResourceEditingService().PrepareResourceCreation(
            compiler,
            context,
            CreateRequest(fullyQualifiedType, "2020-01-01"));
        var textEdit = response.Edit.DocumentChanges!.Single().TextDocumentEdit!.Edits.Single();

        return (ApplyTextEdit(content, context.LineStarts, textEdit), response.ExpectedNodeId, textEdit.NewText);
    }

    /// <summary>Returns the content after replaying a creation, or null if the replay is unavailable.</summary>
    private static string? Replay(string content, string nodeId, string direction, string insertedText)
    {
        var (_, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        var context = new CompilationContext(compilationResult.Compilation);

        var result = new VisualResourceEditingService().PrepareResourceReplays(context, new(
            new TextDocumentIdentifier(DocumentUri.From("main.bicep")),
            [new("operation", nodeId, direction, insertedText)]));

        var edit = result.Replays.Should().ContainSingle().Subject.Edit;
        return edit is null ? null : ApplyTextEdit(content, context.LineStarts, edit);
    }

    private static string ApplyTextEdit(string content, ImmutableArray<int> lineStarts, TextEdit edit) =>
        ApplyTextEdit(content, lineStarts, new VisualResourceReplayTextEdit(edit.Range, edit.NewText));

    private static string ApplyTextEdit(string content, ImmutableArray<int> lineStarts, VisualResourceReplayTextEdit edit)
    {
        var start = PositionHelper.GetOffset(lineStarts, edit.Range.Start);
        var end = PositionHelper.GetOffset(lineStarts, edit.Range.End);

        return content[..start] + edit.NewText + content[end..];
    }

    #endregion

    private static ResourceTypeComponents CreateScopedType(
        string fullyQualifiedType,
        ResourceScope scopes,
        string apiVersion = "2024-01-01",
        ResourceScope readOnlyScopes = ResourceScope.None) =>
        TestTypeHelper.CreateCustomResourceType(
            fullyQualifiedType, apiVersion, TypeSymbolValidationFlags.Default, scopes, readOnlyScopes, ResourceFlags.None);

    private static PrepareVisualResourceCreationParams CreateRequest(string fullyQualifiedType, string apiVersion) =>
        new(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "scope-check",
            new VisualResourceTypeIdentifier(fullyQualifiedType, apiVersion));

    // Mirrors CompilationHelper.Compile's internal implementation, but also returns the BicepCompiler used to
    // build the compilation. The service needs a compiler built from the exact same registrations as the
    // supplied CompilationContext so its internal self-validation recompile of the generated resource
    // declaration succeeds.
    private static (BicepCompiler Compiler, CompilationHelper.CompilationResult Result) CompileWithResourceTypes(
        IEnumerable<ResourceTypeComponents> resourceTypes, string content, string? bicepConfig = null)
    {
        var fileSet = new MockFileSystemTestFileSet();
        fileSet.AddFile("main.bicep", content);
        if (bicepConfig is not null)
        {
            fileSet.AddFile("bicepconfig.json", bicepConfig);
        }

        var compiler = new ServiceBuilder()
            .WithAzResources(resourceTypes)
            .WithFileExplorer(fileSet.FileExplorer)
            .WithFileSystem(fileSet.FileSystem)
            .Build()
            .GetCompiler();

        var compilation = compiler.CreateCompilationWithoutRestore(fileSet.GetUri("main.bicep"));
        return (compiler, CompilationHelper.GetCompilationResult(compilation));
    }

    private static string PrepareAndApply(string content)
    {
        var (compiler, compilationResult) = CompileWithResourceTypes(BuiltInTestTypes.Types, content);
        var context = new CompilationContext(compilationResult.Compilation);
        var service = new VisualResourceEditingService();
        var request = new PrepareVisualResourceCreationParams(
            new VersionedTextDocumentIdentifier { Uri = DocumentUri.From("main.bicep"), Version = 1 },
            "operation",
            new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01"));

        var response = service.PrepareResourceCreation(compiler, context, request);
        return ApplyEdit(content, context.LineStarts, response.Edit);
    }

    // The generated code replacement is always a zero-length insertion, so applying it is a plain string insertion.
    private static string ApplyEdit(string content, ImmutableArray<int> lineStarts, WorkspaceEdit edit)
    {
        var textDocumentEdit = edit.DocumentChanges!.Single().TextDocumentEdit!;
        var textEdit = textDocumentEdit.Edits.Single();
        var offset = PositionHelper.GetOffset(lineStarts, textEdit.Range.Start);

        return content.Insert(offset, textEdit.NewText);
    }
}
