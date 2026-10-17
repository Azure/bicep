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
    public record VisualGraphResult(CanonicalGraph? Graph, string? TargetScope, int ErrorCount);

    /// <summary>
    /// Request sent after the webview has rendered the graph and measured its nodes. The server checks that the
    /// measured topology still matches the live compilation before running MSAGL.
    /// <para>
    /// With <see cref="RootNodeId"/>, only that expanded module is laid out: the measured graph holds the module and
    /// its descendants, the positions place the module's top-left corner at the origin, and the bounds are the
    /// module's size. The client places the result, so expanding one module does not move the rest of the graph.
    /// </para>
    /// </summary>
    [Method("textDocument/visualGraphLayout", Direction.ClientToServer)]
    public record VisualGraphLayoutParams(
        TextDocumentIdentifier TextDocument,
        MeasuredGraph Graph,
        VisualGraphLayoutOptions? Options,
        IReadOnlyList<string>? CollapsedNodeIds = null,
        string? RootNodeId = null) : ITextDocumentIdentifierParams, IRequest<VisualGraphLayoutResult>;

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
    /// Response to a <see cref="VisualGraphNodeSourceParams"/> request. <see cref="Uri"/> is a <c>file:</c> URI, or a
    /// <c>bicep-extsrc:</c> URI for a node inside a registry module's published source. <see cref="Uri"/> and
    /// <see cref="Range"/> are null when the node no longer exists in the live compilation (for example it was
    /// deleted between render and reveal), in which case the client reveals nothing.
    /// </summary>
    public record VisualGraphNodeSourceResult(string? Uri, Range? Range);

    /// <summary>
    /// Request to restore the registry module behind a remote module node whose source is not in the local module
    /// cache, so the node can be expanded. Sent only when the user asks to expand that node. Restoring only fills the
    /// cache: it never edits the document. The client fetches the graph again afterwards.
    /// </summary>
    [Method("textDocument/visualGraphRestoreModuleSource", Direction.ClientToServer)]
    public record VisualGraphRestoreModuleSourceParams(
        TextDocumentIdentifier TextDocument,
        string NodeId) : ITextDocumentIdentifierParams, IRequest<VisualGraphRestoreModuleSourceResult>;

    public static class VisualGraphRestoreModuleSourceStatus
    {
        /// <summary>The module is in the local module cache. The next graph shows its contents, if it was published with source.</summary>
        public const string Restored = "restored";

        /// <summary>The restore failed; <see cref="VisualGraphRestoreModuleSourceResult.Message"/> says why. It can be retried.</summary>
        public const string Failed = "failed";

        /// <summary>The node is no longer a registry module in the live compilation, so nothing was restored.</summary>
        public const string NotFound = "notFound";
    }

    /// <param name="Message">Why the restore failed, when <see cref="Status"/> is <see cref="VisualGraphRestoreModuleSourceStatus.Failed"/>.</param>
    public record VisualGraphRestoreModuleSourceResult(string Status, string? Message = null);
}
