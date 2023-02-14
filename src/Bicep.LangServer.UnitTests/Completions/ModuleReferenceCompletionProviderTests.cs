// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.UnitTests;
using Bicep.LanguageServer.Completions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.LanguageServer.Providers;
using Bicep.Core.Registry;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class ModuleReferenceCompletionProviderTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static ModulesMetadataProvider modulesMetadataProvider = new ModulesMetadataProvider();
        private IServiceClientCredentialsProvider serviceClientCredentialsProvider = StrictMock.Of<IServiceClientCredentialsProvider>().Object;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await modulesMetadataProvider.Initialize();
        }

        [DataTestMethod]
        [DataRow("module test |''", 14)]
        [DataRow("module test ''|", 14)]
        [DataRow("module test '|'", 14)]
        [DataRow("module test '|", 13)]
        [DataRow("module test |", 12)]
        public async Task GetFilteredCompletions_WithBicepRegistryAndTemplateSpecShemaCompletionContext_ReturnsCompletionItems(string inputWithCursors, int expectedEnd)
        {
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            var completionContext = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(modulesMetadataProvider, serviceClientCredentialsProvider);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUri(), completionContext);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("br:");
                    c.Kind.Should().Be(CompletionItemKind.Reference);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Bicep registry schema name");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br:$0'");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                },
                c =>
                {
                    c.Label.Should().Be("br/");
                    c.Kind.Should().Be(CompletionItemKind.Reference);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Bicep registry schema name");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'br/$0'");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                },
                c =>
                {
                    c.Label.Should().Be("ts:");
                    c.Kind.Should().Be(CompletionItemKind.Reference);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Template spec schema name");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'ts:$0'");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                },
                c =>
                {
                    c.Label.Should().Be("ts/");
                    c.Kind.Should().Be(CompletionItemKind.Reference);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().Be("Template spec schema name");
                    c.TextEdit!.TextEdit!.NewText.Should().Be("'ts/$0'");
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                });
        }

        [TestMethod]
        public async Task GetFilteredCompletions_WithInvalidTextInCompletionContext_ReturnsNull()
        {
            var inputWithCursors = "module test 'br:/|'";
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');

            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            var completionContext = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(modulesMetadataProvider, serviceClientCredentialsProvider);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUri(), completionContext);

            completions.Should().BeEmpty();
        }

        [DataTestMethod]
        [DataRow("module test 'br/|'", "'br/public:$0'", 17)]
        [DataRow("module test 'br/|", "'br/public:$0'", 16)]
        public async Task GetFilteredCompletions_WithAliasCompletionContext_ReturnsCompletionItems(string inputWithCursors, string expectedText, int expectedEnd)
        {
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');

            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            var completionContext = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(modulesMetadataProvider, serviceClientCredentialsProvider);
            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUri(), completionContext);

            completions.Should().SatisfyRespectively(
                c =>
                {
                    c.Label.Should().Be("public:");
                    c.Kind.Should().Be(CompletionItemKind.Snippet);
                    c.InsertTextFormat.Should().Be(InsertTextFormat.Snippet);
                    c.InsertText.Should().BeNull();
                    c.Detail.Should().BeNull();
                    c.TextEdit!.TextEdit!.NewText.Should().Be(expectedText);
                    c.TextEdit.TextEdit.Range.Start.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.Start.Character.Should().Be(12);
                    c.TextEdit.TextEdit.Range.End.Line.Should().Be(0);
                    c.TextEdit.TextEdit.Range.End.Character.Should().Be(expectedEnd);
                });
        }

        [DataTestMethod]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/|'", "app/dapr-containerapp", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:$0'", 0, 12, 0, 41)]
        [DataRow("module test 'br:mcr.microsoft.com/bicep/|", "app/dapr-containerapp", "'br:mcr.microsoft.com/bicep/app/dapr-containerapp:$0'", 0, 12, 0, 40)]
        [DataRow("module test 'br/public:|'", "app/dapr-containerapp", "'br/public:app/dapr-containerapp:$0'", 0, 12, 0, 24)]
        [DataRow("module test 'br/public:|", "app/dapr-containerapp", "'br/public:app/dapr-containerapp:$0'", 0, 12, 0, 23)]

        public async Task GetFilteredCompletions_WithPublicMcrModuleRegistryCompletionContext_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel,
            string expectedCompletionText,
            int startLine,
            int startCharacter,
            int endLine,
            int endCharacter)
        {
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            var completionContext = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(modulesMetadataProvider, serviceClientCredentialsProvider);

            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUri(), completionContext);

            completions.Should().Contain(
                x => x.Label == expectedLabel &&
                x.Kind == CompletionItemKind.Snippet &&
                x.InsertText == null &&
                x.TextEdit!.TextEdit!.NewText == expectedCompletionText &&
                x.TextEdit!.TextEdit!.Range.Start.Line == startLine  &&
                x.TextEdit!.TextEdit!.Range.Start.Character == startCharacter &&
                x.TextEdit!.TextEdit!.Range.End.Line == endLine &&
                x.TextEdit!.TextEdit!.Range.End.Character == endCharacter);
        }

        [DataTestMethod]
        [DataRow("module test 'br/public:app/dapr-containerapp:|'", "1.0.1", "'br/public:app/dapr-containerapp:1.0.1'$0", 0, 12, 0, 46)]
        [DataRow("module test 'br/public:app/dapr-containerapp:|", "1.0.1", "'br/public:app/dapr-containerapp:1.0.1'$0", 0, 12, 0, 45)]
        public async Task GetFilteredCompletions_WithMcrVersionCompletionContext_ReturnsCompletionItems(
            string inputWithCursors,
            string expectedLabel,
            string expectedCompletionText,
            int startLine,
            int startCharacter,
            int endLine,
            int endCharacter)
        {
            var (bicepFileContents, cursors) = ParserHelper.GetFileWithCursors(inputWithCursors, '|');
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "input.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var bicepCompilationManager = BicepCompilationManagerHelper.CreateCompilationManager(documentUri, bicepFileContents, true);
            var compilation = bicepCompilationManager.GetCompilation(documentUri)!.Compilation;
            var completionContext = BicepCompletionContext.Create(BicepTestConstants.Features, compilation, cursors[0]);
            var moduleReferenceCompletionProvider = new ModuleReferenceCompletionProvider(modulesMetadataProvider, serviceClientCredentialsProvider);

            var completions = await moduleReferenceCompletionProvider.GetFilteredCompletions(documentUri.ToUri(), completionContext);

            completions.Should().Contain(
                x => x.Label == expectedLabel &&
                x.Kind == CompletionItemKind.Snippet &&
                x.InsertText == null &&
                x.TextEdit!.TextEdit!.NewText == expectedCompletionText &&
                x.TextEdit!.TextEdit!.Range.Start.Line == startLine &&
                x.TextEdit!.TextEdit!.Range.Start.Character == startCharacter &&
                x.TextEdit!.TextEdit!.Range.End.Line == endLine &&
                x.TextEdit!.TextEdit!.Range.End.Character == endCharacter);
        }
    }
}
