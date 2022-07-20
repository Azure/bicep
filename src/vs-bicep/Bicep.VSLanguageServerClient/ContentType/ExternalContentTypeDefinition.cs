// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.VSLanguageServerClient.ContentType
{
    public sealed class ExternalContentTypeDefinition
    {
        /// <summary>
        /// Code remote content type name. Lights up all TextMate features (colorization, brace completion, folding ranges).
        /// </summary>
        public const string CodeRemoteContentTypeName = "code-languageserver-preview";
    }
}
