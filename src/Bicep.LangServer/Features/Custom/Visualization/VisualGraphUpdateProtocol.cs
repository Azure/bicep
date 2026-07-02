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
    /// Request sent by the webview (via the extension) asking the server to reconcile the topology/metadata
    /// it currently displays with the server's latest graph. Layout is intentionally not computed here: when
    /// topology changes, the client first renders/measures nodes and then sends <see cref="VisualGraphLayoutParams"/>.
    /// <para>
    /// Handled by <see cref="VisualGraphUpdateHandler"/>. The visualizer always routes through this path.
    /// </para>
    /// </summary>
    [Method("textDocument/visualGraphUpdate", Direction.ClientToServer)]
    public record VisualGraphUpdateParams(
        TextDocumentIdentifier TextDocument,
        RenderedGraph? Current) : ITextDocumentIdentifierParams, IRequest<VisualGraphUpdateResult>;

    /// <summary>
    /// Response to a <see cref="VisualGraphUpdateParams"/> request: a complete, ordered topology/metadata
    /// delta transforming the submitted graph into the server's latest graph. The client decides whether the
    /// patches may affect rendered size and whether to send a follow-up <see cref="VisualGraphLayoutParams"/> request.
    /// </summary>
    public record VisualGraphUpdateResult(IReadOnlyList<GraphPatch> Patches);

    /// <summary>
    /// Request sent after the webview has applied graph update patches and measured actual node sizes. The
    /// server validates that the measured topology still matches the live compilation before running MSAGL.
    /// </summary>
    [Method("textDocument/visualGraphLayout", Direction.ClientToServer)]
    public record VisualGraphLayoutParams(
        TextDocumentIdentifier TextDocument,
        RenderedGraph Current,
        VisualGraphLayoutOptions? Options) : ITextDocumentIdentifierParams, IRequest<VisualGraphLayoutResult>;

    public static class VisualGraphLayoutStatus
    {
        public const string Ok = "ok";

        public const string GraphChanged = "graphChanged";

        public const string LayoutFailed = "layoutFailed";
    }

    /// <summary>
    /// Response to a <see cref="VisualGraphLayoutParams"/> request. Successful responses contain
    /// <see cref="GraphPatch.SetNodeLayout"/> patches and, when available, one <see cref="GraphPatch.SetGraphBounds"/> patch.
    /// </summary>
    public record VisualGraphLayoutResult(string Status, IReadOnlyList<GraphPatch> Patches);

    /// <summary>
    /// Request to resolve a node's source location on demand (for example when the user double-clicks a node
    /// to reveal it). The canonical graph carries no source location, so the webview asks for it by node id
    /// only when it actually needs to reveal, keeping volatile range/file-path data out of the graph diff.
    /// </summary>
    [Method("textDocument/visualGraphNodeSource", Direction.ClientToServer)]
    public record VisualGraphNodeSourceParams(
        TextDocumentIdentifier TextDocument,
        string NodeId) : ITextDocumentIdentifierParams, IRequest<VisualGraphNodeSourceResult>;

    /// <summary>
    /// Response to a <see cref="VisualGraphNodeSourceParams"/> request. <see cref="Found"/> is false when the
    /// node no longer exists in the live compilation (for example it was deleted between render and reveal),
    /// in which case <see cref="FilePath"/>/<see cref="Range"/> are null and the client reveals nothing.
    /// </summary>
    public record VisualGraphNodeSourceResult(bool Found, string? FilePath, Range? Range);
}
