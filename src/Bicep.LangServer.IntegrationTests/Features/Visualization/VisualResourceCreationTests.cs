// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Features.Custom.Visualization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class VisualResourceCreationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task VisualResourceTypes_ReturnsAlphabeticallyOrderedCatalogOnceUntilItChanges()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new VisualResourceTypesParams(new TextDocumentIdentifier(helper.MainUri), KnownCatalogId: null),
                default);

            result.CatalogId.Should().NotBeNullOrEmpty();
            var resourceTypes = result.ResourceTypes!;
            resourceTypes.Select(entry => entry.FullyQualifiedType).Should().Contain(
                "Test.Rp/basicTests", "Test.Rp/readWriteTests", "Test.Rp/discriminatorTests");

            // Entries are ordered by fully-qualified type so the client can group them without re-sorting.
            resourceTypes.Select(entry => entry.FullyQualifiedType).Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase);

            var basicTest = resourceTypes.Should().ContainSingle(entry => entry.FullyQualifiedType == "Test.Rp/basicTests").Subject;
            basicTest.ApiVersion.Should().Be("2020-01-01");
            basicTest.IsPreview.Should().BeFalse();

            var unchanged = await client.SendRequest(
                new VisualResourceTypesParams(new TextDocumentIdentifier(helper.MainUri), result.CatalogId),
                default);

            unchanged.CatalogId.Should().Be(result.CatalogId);
            unchanged.ResourceTypes.Should().BeNull();
        }
        [TestMethod]
        public async Task PrepareVisualResource_HappyPath_ReturnsVersionedEditThatAppliesToCurrentDocumentVersion()
        {
            // Uses a document with no pre-existing declarations so the generated symbolic name is the
            // unsuffixed base name; collision-suffix behavior is covered separately below.
            using var helper = await StartServerAndOpenAsync(string.Empty);
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new PrepareVisualResourceCreationParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01")),
                default);

            result.OperationId.Should().Be("operation-1");
            result.ExpectedNodeId.Should().Be("basicTest");
            result.UnresolvedRequiredProperties.Should().BeEmpty();

            var textDocumentEdit = result.Edit.DocumentChanges.Should().ContainSingle().Subject.TextDocumentEdit;
            textDocumentEdit.Should().NotBeNull();
            textDocumentEdit!.TextDocument.Uri.Should().Be(helper.MainUri);
            textDocumentEdit.TextDocument.Version.Should().Be(1);

            var updatedContent = ApplyEdit(helper.MainContent, result.Edit);
            updatedContent.ReplaceLineEndings("\n").Should().Be("""
                resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
                  name: 'basicTest'
                }
                """);
        }

        [TestMethod]
        public async Task PrepareVisualResource_SymbolicNameCollision_AvoidsExistingDeclaration()
        {
            // The document already declares a top-level "basicTest" symbol, so the generated name must avoid
            // colliding with it. Symbolic-name generation only considers the current (live) document's
            // declarations - not other in-flight/unapplied prepare requests - so two requests issued against
            // the same unmodified document deterministically produce the same suffixed name both times; the
            // client is expected to apply the returned edit (advancing the document version) before the next
            // request if it wants a further-incremented suffix.
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var first = await client.SendRequest(
                new PrepareVisualResourceCreationParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01")),
                default);
            first.ExpectedNodeId.Should().Be("basicTest1");

            var second = await client.SendRequest(
                new PrepareVisualResourceCreationParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-2",
                    new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01")),
                default);
            second.ExpectedNodeId.Should().Be("basicTest1");
        }

        [TestMethod]
        public async Task PrepareVisualResource_DiscriminatedType_ReportsDiscriminatorKeyAsUnresolved()
        {
            using var helper = await StartServerAndOpenAsync(string.Empty);
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new PrepareVisualResourceCreationParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/discriminatorTests", "2020-01-01")),
                default);

            result.UnresolvedRequiredProperties.Should().Equal("kind");
            ApplyEdit(helper.MainContent, result.Edit).ReplaceLineEndings("\n").Should().Be("""
                resource discriminatorTest 'Test.Rp/discriminatorTests@2020-01-01' = {
                  kind:
                }
                """);
        }

        [TestMethod]
        public async Task PrepareVisualResource_UnknownResourceType_ThrowsRpcError()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            Func<Task> request = async () => await client.SendRequest(
                new PrepareVisualResourceCreationParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/doesNotExist", "2020-01-01")),
                default);

            var exception = await request.Should().ThrowAsync<JsonRpcException>()
                .WithMessage("Resource type \"Test.Rp/doesNotExist@2020-01-01\" was not found.");
            exception.Which.Error.Should().BeEmpty();
        }

        [TestMethod]
        public async Task PrepareVisualResourceReplay_ReturnsAnUndoEditOrNullPerCreation()
        {
            var insertedText = """
                resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
                  name: 'basicTest'
                }
                """;
            using var helper = await StartServerAndOpenAsync("param location string\n\n" + insertedText);
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new PrepareVisualResourceReplayParams(
                    new TextDocumentIdentifier(helper.MainUri),
                    [
                        new("undo-op", "basicTest", VisualResourceReplayDirection.Undo, "\n\n" + insertedText),
                        new("redo-op", "basicTest", VisualResourceReplayDirection.Redo, "\n\n" + insertedText),
                    ]),
                default);

            result.Replays.Select(replay => replay.OperationId).Should().Equal("undo-op", "redo-op");
            var undo = result.Replays[0].Edit;
            undo.Should().NotBeNull();
            undo!.NewText.Should().BeEmpty();
            undo.Range.Should().Be(new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(0, 21, 4, 1));
            result.Replays[1].Edit.Should().BeNull();
        }

        private async Task<TestServer> StartServerAndOpenAsync(string? mainContent = null)
        {
            mainContent ??= """
                resource basicTest 'Test.Rp/basicTests@2020-01-01' = {
                  name: 'basicTest'
                }
                """;
            var mainUri = DocumentUri.From("/main.bicep");

            var helper = await LanguageServerHelper.StartServerWithText(
                this.TestContext,
                mainContent,
                mainUri,
                services => services.WithNamespaceProvider(BuiltInTestTypes.Create()));

            return new TestServer(helper, mainUri, mainContent);
        }

        private sealed class TestServer : IDisposable
        {
            public TestServer(LanguageServerHelper helper, DocumentUri mainUri, string mainContent)
            {
                this.Helper = helper;
                this.MainUri = mainUri;
                this.MainContent = mainContent;
            }

            public LanguageServerHelper Helper { get; }

            public DocumentUri MainUri { get; }

            public string MainContent { get; }

            public void Dispose() => this.Helper.Dispose();
        }

        // The generated code replacement is always a zero-length insertion appended at the end of the
        // document (see GeneratedResourceDeclaration), so applying it is a plain
        // string insertion at the offset the single TextEdit's range describes.
        private static string ApplyEdit(string content, WorkspaceEdit edit)
        {
            var textDocumentEdit = edit.DocumentChanges!.Single().TextDocumentEdit!;
            var textEdit = textDocumentEdit.Edits.Single();

            return content.Insert(content.Length, textEdit.NewText);
        }
    }
}
