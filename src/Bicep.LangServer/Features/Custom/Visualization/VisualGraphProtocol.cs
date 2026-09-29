// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Returns the document's graph as built from the live compilation. Layout is not computed here: when the
    /// graph changes, the client first renders and measures the nodes, then sends <see cref="VisualGraphLayoutParams"/>.
    /// </summary>
    [Method("textDocument/visualGraph", Direction.ClientToServer)]
    public record VisualGraphParams(
        TextDocumentIdentifier TextDocument) : ITextDocumentIdentifierParams, IRequest<VisualGraphResult>;

    /// <summary>
    /// The whole graph, rather than a delta, so a response never depends on what the client showed before.
    /// <see cref="Graph"/> is null when the document has not been compiled yet, in which case the client keeps
    /// what it shows until the next document change.
    /// </summary>
    public record VisualGraphResult(CanonicalGraph? Graph, string? TargetScope);

    /// <summary>
    /// Request sent after the webview has rendered the graph and measured its nodes. The server checks that the
    /// measured topology still matches the live compilation before running MSAGL.
    /// </summary>
    [Method("textDocument/visualGraphLayout", Direction.ClientToServer)]
    public record VisualGraphLayoutParams(
        TextDocumentIdentifier TextDocument,
        MeasuredGraph Graph,
        VisualGraphLayoutOptions? Options) : ITextDocumentIdentifierParams, IRequest<VisualGraphLayoutResult>;

    public static class VisualGraphLayoutStatus
    {
        public const string Ok = "ok";

        public const string GraphChanged = "graphChanged";

        public const string LayoutFailed = "layoutFailed";
    }

    /// <summary>
    /// Response to a <see cref="VisualGraphLayoutParams"/> request. When <see cref="Status"/> is
    /// <see cref="VisualGraphLayoutStatus.Ok"/>, <see cref="Positions"/> holds the nodes the engine positioned and
    /// <see cref="Bounds"/> the extent of the whole graph; otherwise both are empty.
    /// </summary>
    public record VisualGraphLayoutResult(string Status, IReadOnlyList<NodePosition> Positions, GraphBounds? Bounds);

    /// <summary>
    /// Request to resolve a node's source location on demand (for example when the user double-clicks a node
    /// to reveal it). The canonical graph carries no source location, so the webview asks for it by node id
    /// only when it actually needs to reveal, keeping volatile range/file-path data out of the graph.
    /// </summary>
    [Method("textDocument/visualGraphNodeSource", Direction.ClientToServer)]
    public record VisualGraphNodeSourceParams(
        TextDocumentIdentifier TextDocument,
        string NodeId) : ITextDocumentIdentifierParams, IRequest<VisualGraphNodeSourceResult>;

    /// <summary>
    /// Response to a <see cref="VisualGraphNodeSourceParams"/> request. <see cref="FilePath"/> and <see cref="Range"/>
    /// are null when the node no longer exists in the live compilation (for example it was deleted between render
    /// and reveal), in which case the client reveals nothing.
    /// </summary>
    public record VisualGraphNodeSourceResult(string? FilePath, Range? Range);
}
