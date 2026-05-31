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
    /// This is the contract only; the handler is introduced in a later change. Until then no handler is
    /// registered, so sending this request is a no-op and the existing deployment graph path is unaffected.
    /// </para>
    /// </summary>
    [Method("textDocument/visualizerGraphUpdate", Direction.ClientToServer)]
    public record VisualizerGraphUpdateParams(
        TextDocumentIdentifier TextDocument,
        ClientGraph? Current) : ITextDocumentIdentifierParams, IRequest<VisualizerGraphUpdateResult>;

    /// <summary>
    /// Response to a <see cref="VisualizerGraphUpdateParams"/> request: a complete, ordered delta transforming
    /// the submitted graph into the server's latest graph. An empty list means nothing changed.
    /// </summary>
    public record VisualizerGraphUpdateResult(IReadOnlyList<GraphPatch> Patches);
}
