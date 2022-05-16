// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace Bicep.VSLanguageServerClient.RequestHandler
{
    /// <summary>
    /// Top level type for LSP request handler.
    /// </summary>
    public interface IRequestHandler
    {
    }

    public interface IRequestHandler<RequestType, ResponseType> : IRequestHandler where RequestType : class
    {
        /// <summary>
        /// Handles an LSP request.
        /// </summary>
        /// <param name="request">the lsp request.</param>
        /// <param name="clientCapabilities">the client capabilities for the request.</param>
        /// <param name="cancellationToken">a cancellation token.</param>
        /// <returns>the LSP response.</returns>
        Task<ResponseType?> HandleRequestAsync(RequestType request, ClientCapabilities clientCapabilities, CancellationToken cancellationToken);
    }
}
