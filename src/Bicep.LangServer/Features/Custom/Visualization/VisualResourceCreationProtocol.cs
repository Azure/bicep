// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

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

    public static class VisualResourceReplayDirection
    {
        public const string Undo = "undo";

        public const string Redo = "redo";
    }

    /// <summary>
    /// Asks whether resource creations made by the visual designer can still be undone or redone exactly, and for
    /// the edit that would do it. The client sends one entry per creation in its history, each with the text the
    /// creation inserted, and applies a returned edit itself (subject to its own version checks).
    /// </summary>
    [Method("textDocument/prepareVisualResourceReplay", Direction.ClientToServer)]
    public record PrepareVisualResourceReplayParams(
        TextDocumentIdentifier TextDocument,
        IReadOnlyList<VisualResourceReplayQuery> Replays) : ITextDocumentIdentifierParams, IRequest<PrepareVisualResourceReplayResult>;

    /// <param name="OperationId">Identifies the creation. Echoed back in the result.</param>
    /// <param name="NodeId">The graph node the creation produced, which is the resource's symbolic name.</param>
    /// <param name="Direction"><see cref="VisualResourceReplayDirection.Undo"/> or <see cref="VisualResourceReplayDirection.Redo"/>.</param>
    /// <param name="InsertedText">The text the creation inserted, including its surrounding newlines, as the document contains it.</param>
    public record VisualResourceReplayQuery(
        string OperationId,
        string NodeId,
        string Direction,
        string InsertedText);

    public record PrepareVisualResourceReplayResult(IReadOnlyList<VisualResourceReplayEdit> Replays);

    /// <param name="Edit">
    /// The edit that undoes or redoes the creation against the current document, or null when it cannot be replayed
    /// exactly: the declaration was edited or is referenced (undo), or its symbolic name is taken (redo).
    /// </param>
    public record VisualResourceReplayEdit(
        string OperationId,
        VisualResourceReplayTextEdit? Edit);

    /// <summary>
    /// A text replacement in the document. Not an LSP <see cref="TextEdit"/>, whose JSON converter cannot read an absent edit.
    /// </summary>
    public record VisualResourceReplayTextEdit(
        Range Range,
        string NewText);
}
