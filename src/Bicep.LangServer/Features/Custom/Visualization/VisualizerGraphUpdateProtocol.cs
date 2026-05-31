// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Request sent by the webview (via the extension) asking the server to reconcile the graph it
    /// currently displays with the server's latest graph. The server returns a complete patch delta.
    /// <para>
    /// Handled by <see cref="VisualizerGraphUpdateHandler"/>. The handler always answers (it is not gated by
    /// the feature flag); the flag only controls whether the extension routes the visualizer through this path,
    /// so the existing deployment graph path is unaffected until the extension opts in.
    /// </para>
    /// </summary>
    [Method("textDocument/visualizerGraphUpdate", Direction.ClientToServer)]
    public record VisualizerGraphUpdateParams(
        TextDocumentIdentifier TextDocument,
        RenderedGraph? Current) : ITextDocumentIdentifierParams, IRequest<VisualizerGraphUpdateResult>;

    /// <summary>
    /// Response to a <see cref="VisualizerGraphUpdateParams"/> request: a complete, ordered delta transforming
    /// the submitted graph into the server's latest graph. An empty list means nothing changed.
    /// </summary>
    public record VisualizerGraphUpdateResult(IReadOnlyList<GraphPatch> Patches);
}
