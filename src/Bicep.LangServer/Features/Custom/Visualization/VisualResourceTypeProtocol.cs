// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// A single entry in the resource type catalog returned by <see cref="VisualResourceTypesParams"/>.
    /// </summary>
    public record VisualResourceTypeCatalogEntry(
        string FullyQualifiedType,
        string ApiVersion,
        bool IsPreview);

    /// <summary>
    /// Returns every resource type deployable at the document's target scope. The catalog is identified by
    /// <c>CatalogId</c>, which changes when the document's type provider or target scope does.
    /// </summary>
    /// <param name="KnownCatalogId">The catalog the client already holds, if any.</param>
    [Method("textDocument/visualResourceTypes", Direction.ClientToServer)]
    public record VisualResourceTypesParams(
        TextDocumentIdentifier TextDocument,
        string? KnownCatalogId) : ITextDocumentIdentifierParams, IRequest<VisualResourceTypesResult>;

    /// <param name="ResourceTypes">Every type, sorted by name, or null when the client already holds this catalog.</param>
    public record VisualResourceTypesResult(
        string CatalogId,
        IReadOnlyList<VisualResourceTypeCatalogEntry>? ResourceTypes);

    [Method("textDocument/visualResourceTypeVersions", Direction.ClientToServer)]
    public record VisualResourceTypeVersionsParams(
        TextDocumentIdentifier TextDocument,
        string FullyQualifiedType) : ITextDocumentIdentifierParams, IRequest<VisualResourceTypeVersionsResult>;

    public record VisualResourceTypeVersionsResult(
        string CatalogId,
        IReadOnlyList<string> ApiVersions);
}
