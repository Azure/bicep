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

    [Method("textDocument/visualResourceTypes", Direction.ClientToServer)]
    public record VisualResourceTypesParams(
        TextDocumentIdentifier TextDocument,
        string? Query,
        bool IncludePreview,
        int PageSize,
        string? ContinuationToken) : ITextDocumentIdentifierParams, IRequest<VisualResourceTypesResult>;

    public record VisualResourceTypesResult(
        IReadOnlyList<VisualResourceTypeCatalogEntry> Items,
        string? ContinuationToken);

    [Method("textDocument/prepareVisualResource", Direction.ClientToServer)]
    public record PrepareVisualResourceParams(
        VersionedTextDocumentIdentifier TextDocument,
        string OperationId,
        VisualResourceTypeIdentifier ResourceType) : IRequest<PrepareVisualResourceResult>;

    public record PrepareVisualResourceResult(
        string OperationId,
        string ExpectedNodeId,
        string SymbolicName,
        IReadOnlyList<string> UnresolvedRequiredProperties,
        WorkspaceEdit Edit);
}
