// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Thrown by <see cref="IVisualResourceCreationService"/> when a request cannot be satisfied. Handlers are expected
    /// to translate this into an RPC error response.
    /// </summary>
    public sealed class VisualResourceCreationException : Exception
    {
        public VisualResourceCreationException(string message) : base(message)
        {
        }
    }
}
