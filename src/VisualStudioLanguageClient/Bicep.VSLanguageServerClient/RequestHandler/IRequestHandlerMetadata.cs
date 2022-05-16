// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.VSLanguageServerClient.RequestHandler
{
    public interface IRequestHandlerMetadata
    {
        /// <summary>
        /// Name of the LSP method to handle.
        /// </summary>
        string MethodName { get; }
    }
}
