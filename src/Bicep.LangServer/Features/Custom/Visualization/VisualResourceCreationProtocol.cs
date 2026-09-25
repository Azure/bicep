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
    /// Identifies a specific resource type and API version selected from the Resource Palette.
    /// </summary>
    public record VisualResourceTypeIdentifier(
        string FullyQualifiedType,
        string ApiVersion);

    public record VisualResourceTypeNamespace(
        string Name,
        int ResourceTypeCount);

    [Method("textDocument/visualResourceTypeNamespaces", Direction.ClientToServer)]
    public record VisualResourceTypeNamespacesParams(
        TextDocumentIdentifier TextDocument) : ITextDocumentIdentifierParams, IRequest<VisualResourceTypeNamespacesResult>;

    public record VisualResourceTypeNamespacesResult(
        string CatalogId,
        IReadOnlyList<VisualResourceTypeNamespace> Namespaces);

    [Method("textDocument/visualResourceTypes", Direction.ClientToServer)]
    public record VisualResourceTypesParams(
        TextDocumentIdentifier TextDocument,
        string? ProviderNamespace,
        string? Query,
        int PageSize,
        string? ContinuationToken) : ITextDocumentIdentifierParams, IRequest<VisualResourceTypesResult>;

    public record VisualResourceTypesResult(
        string CatalogId,
        IReadOnlyList<VisualResourceTypeCatalogEntry> Items,
        string? ContinuationToken);

    [Method("textDocument/visualResourceTypeVersions", Direction.ClientToServer)]
    public record VisualResourceTypeVersionsParams(
        TextDocumentIdentifier TextDocument,
        string FullyQualifiedType) : ITextDocumentIdentifierParams, IRequest<VisualResourceTypeVersionsResult>;

    public record VisualResourceTypeVersionsResult(
        string CatalogId,
        IReadOnlyList<string> ApiVersions);

    [Method("textDocument/prepareVisualResource", Direction.ClientToServer)]
    public record CreateResourceDeclarationInsertionParams(
        VersionedTextDocumentIdentifier TextDocument,
        string OperationId,
        VisualResourceTypeIdentifier ResourceType) : IRequest<ResourceDeclarationInsertion>;

    public record ResourceDeclarationInsertion(
        string OperationId,
        string ExpectedNodeId,
        string SymbolicName,
        IReadOnlyList<string> UnresolvedRequiredProperties,
        WorkspaceEdit Edit);
}
