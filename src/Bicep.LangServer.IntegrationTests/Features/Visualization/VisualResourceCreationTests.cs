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
        [TestMethod]
        public async Task VisualResourceTypeNamespaces_ReturnsProviderCounts()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;
            var result = await client.SendRequest(
                new VisualResourceTypeNamespacesParams(new TextDocumentIdentifier(helper.MainUri), IncludePreview: false),
                default);

            result.CatalogId.Should().NotBeNullOrEmpty();
            result.Namespaces.Should().NotBeEmpty();
            result.Namespaces.Should().BeInAscendingOrder(entry => entry.Name);
            result.Namespaces.Should().OnlyContain(entry => entry.ResourceTypeCount > 0);
        }

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public async Task VisualResourceTypes_ReturnsAlphabeticallyOrderedCatalog()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new VisualResourceTypesParams(new TextDocumentIdentifier(helper.MainUri), ProviderNamespace: null, Query: null, IncludePreview: true, PageSize: 50, ContinuationToken: null),
                default);

            result.Should().NotBeNull();
            result.ContinuationToken.Should().BeNull();
            result.Items.Select(entry => entry.FullyQualifiedType).Should().Contain(
                "Test.Rp/basicTests", "Test.Rp/readWriteTests", "Test.Rp/discriminatorTests");

            // Entries are ordered by fully-qualified type so the client can render an alphabetically sorted tree
            // without re-sorting.
            result.Items.Select(entry => entry.FullyQualifiedType)
                .Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase);

            var basicTest = result.Items.Should().ContainSingle(entry => entry.FullyQualifiedType == "Test.Rp/basicTests").Subject;
            basicTest.ApiVersion.Should().Be("2020-01-01");
            basicTest.IsPreview.Should().BeFalse();
        }

        [TestMethod]
        public async Task VisualResourceTypes_WithQuery_FiltersCaseInsensitively()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new VisualResourceTypesParams(new TextDocumentIdentifier(helper.MainUri), ProviderNamespace: null, Query: "READWRITE", IncludePreview: true, PageSize: 50, ContinuationToken: null),
                default);

            result.Items.Should().ContainSingle().Which.FullyQualifiedType.Should().Be("Test.Rp/readWriteTests");
        }

        [TestMethod]
        public async Task VisualResourceTypes_WithSmallPageSize_PagesThroughEntireCatalogViaContinuationToken()
        {
            using var helper = await StartServerAndOpenAsync();
            var client = helper.Helper.Client;

            var full = await client.SendRequest(
                new VisualResourceTypesParams(new TextDocumentIdentifier(helper.MainUri), ProviderNamespace: null, Query: null, IncludePreview: true, PageSize: 200, ContinuationToken: null),
                default);

            var seen = new List<VisualResourceTypeCatalogEntry>();
            string? continuationToken = null;
            do
            {
                var page = await client.SendRequest(
                    new VisualResourceTypesParams(new TextDocumentIdentifier(helper.MainUri), ProviderNamespace: null, Query: null, IncludePreview: true, PageSize: 1, ContinuationToken: continuationToken),
                    default);

                page.Items.Should().HaveCountLessOrEqualTo(1);
                seen.AddRange(page.Items);
                continuationToken = page.ContinuationToken;
            } while (continuationToken is not null);

            // Paging one entry at a time via the continuation token must reproduce the same set and order as a
            // single unpaginated request.
            seen.Should().Equal(full.Items);
        }

        [TestMethod]
        public async Task PrepareVisualResource_HappyPath_ReturnsVersionedEditThatAppliesToCurrentDocumentVersion()
        {
            // Uses a document with no pre-existing declarations so the generated symbolic name is the
            // unsuffixed base name; collision-suffix behavior is covered separately below.
            using var helper = await StartServerAndOpenAsync(string.Empty);
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new PrepareVisualResourceParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01")),
                default);

            result.OperationId.Should().Be("operation-1");
            result.SymbolicName.Should().Be("basicTest");
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
                new PrepareVisualResourceParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01")),
                default);
            first.SymbolicName.Should().Be("basicTest1");

            var second = await client.SendRequest(
                new PrepareVisualResourceParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-2",
                    new VisualResourceTypeIdentifier("Test.Rp/basicTests", "2020-01-01")),
                default);
            second.SymbolicName.Should().Be("basicTest1");
        }

        [TestMethod]
        public async Task PrepareVisualResource_DiscriminatedType_ReportsDiscriminatorKeyAsUnresolved()
        {
            using var helper = await StartServerAndOpenAsync(string.Empty);
            var client = helper.Helper.Client;

            var result = await client.SendRequest(
                new PrepareVisualResourceParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/discriminatorTests", "2020-01-01")),
                default);

            result.UnresolvedRequiredProperties.Should().Equal("kind");
            var updatedContent = ApplyEdit(helper.MainContent, result.Edit);
            updatedContent.ReplaceLineEndings("\n").Should().Be("""
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
                new PrepareVisualResourceParams(
                    new VersionedTextDocumentIdentifier { Uri = helper.MainUri, Version = 1 },
                    "operation-1",
                    new VisualResourceTypeIdentifier("Test.Rp/doesNotExist", "2020-01-01")),
                default);

            var exception = await request.Should().ThrowAsync<JsonRpcException>()
                .WithMessage("Resource type \"Test.Rp/doesNotExist@2020-01-01\" was not found.");
            exception.Which.Error.Should().BeEmpty();
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
        // document (see VisualResourceCreationService.GetInsertContext), so applying it is a plain
        // string insertion at the offset the single TextEdit's range describes.
        private static string ApplyEdit(string content, WorkspaceEdit edit)
        {
            var textDocumentEdit = edit.DocumentChanges!.Single().TextDocumentEdit!;
            var textEdit = textDocumentEdit.Edits.Single();

            return content.Insert(content.Length, textEdit.NewText);
        }
    }
}
