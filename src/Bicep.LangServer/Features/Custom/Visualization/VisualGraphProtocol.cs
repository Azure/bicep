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
    /// The whole graph, rather than a delta, so a response never depends on what the client showed before, and the
    /// document facts shown beside it. <see cref="Graph"/> is null when the document has not been compiled yet, in
    /// which case the client keeps what it shows until the next document change.
    /// </summary>
    /// <param name="ErrorCount">The errors reported for the document, including those that belong to no node.</param>
    /// <param name="Revision">
    /// Identifies the graph's topology (see <see cref="CanonicalGraph.GetRevision"/>). The client sends it back
    /// with a layout request, so the server can tell whether the graph it laid out is still current.
    /// </param>
    public record VisualGraphResult(CanonicalGraph? Graph, string? TargetScope, int ErrorCount, string? Revision = null);

    /// <summary>
    /// Request sent after the webview has rendered the graph and measured its nodes. <see cref="Graph"/> is what the
    /// webview renders, which may omit nodes it hides (such as the children of a collapsed module), and is laid out
    /// as sent. <see cref="Revision"/> is the revision of the graph it was rendered from; when the live compilation
    /// has a different topology the server answers <see cref="VisualGraphLayoutStatus.GraphChanged"/>.
    /// </summary>
    [Method("textDocument/visualGraphLayout", Direction.ClientToServer)]
    public record VisualGraphLayoutParams(
        TextDocumentIdentifier TextDocument,
        MeasuredGraph Graph,
        VisualGraphLayoutOptions? Options,
        string? Revision) : ITextDocumentIdentifierParams, IRequest<VisualGraphLayoutResult>;

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
    /// Response to a <see cref="VisualGraphNodeSourceParams"/> request. <see cref="Uri"/> is the document declaring the
    /// node: a file, or a <c>bicep-extsrc:</c> view of a registry module's published source. <see cref="Uri"/> and
    /// <see cref="Range"/> are null when the node no longer exists in the live compilation (for example it was deleted
    /// between render and reveal), in which case the client reveals nothing.
    /// </summary>
    public record VisualGraphNodeSourceResult(string? Uri, Range? Range);

    /// <summary>
    /// Request to restore the registry module behind a node whose <see cref="Models.GraphNode.SourceStatus"/> is
    /// <c>unrestored</c> or <c>error</c> into the local module cache, sent when the user expands it. It completes once the
    /// restore has finished, successfully or not, and never edits or recompiles the document: the client then requests
    /// the graph, which shows the module's children or its new status. Any other node is ignored.
    /// </summary>
    [Method("textDocument/visualGraphRestoreModule", Direction.ClientToServer)]
    public record VisualGraphRestoreModuleParams(
        TextDocumentIdentifier TextDocument,
        string NodeId) : ITextDocumentIdentifierParams, IRequest<VisualGraphRestoreModuleResult>;

    /// <summary>
    /// Response to a <see cref="VisualGraphRestoreModuleParams"/> request. <see cref="Restored"/> says whether the module
    /// is now in the local module cache; it is for logging, since the next graph says what the module shows.
    /// </summary>
    public record VisualGraphRestoreModuleResult(bool Restored);
}
