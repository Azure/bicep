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
    /// Handled by <see cref="VisualGraphUpdateHandler"/>. The handler always answers (it is not gated by
    /// the feature flag); the flag only controls whether the extension routes the visual graph through this path,
    /// so the existing deployment graph path is unaffected until the extension opts in.
    /// </para>
    /// </summary>
    [Method("textDocument/visualGraphUpdate", Direction.ClientToServer)]
    public record VisualGraphUpdateParams(
        TextDocumentIdentifier TextDocument,
        RenderedGraph? Current) : ITextDocumentIdentifierParams, IRequest<VisualGraphUpdateResult>;

    /// <summary>
    /// Response to a <see cref="VisualGraphUpdateParams"/> request: a complete, ordered delta transforming
    /// the submitted graph into the server's latest graph. An empty list means nothing changed.
    /// </summary>
    public record VisualGraphUpdateResult(IReadOnlyList<GraphPatch> Patches);
}
