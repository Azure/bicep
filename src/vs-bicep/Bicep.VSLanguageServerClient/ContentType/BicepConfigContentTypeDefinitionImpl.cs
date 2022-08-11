// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Bicep.VSLanguageServerClient.ContentType
{
    public class BicepConfigContentTypeDefinitionImpl
    {
        /// <summary>
        /// Exports the bicep config content type.
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(BicepLanguageServerClientConstants.BicepConfigContentType)]
        [BaseDefinition(BicepLanguageServerClientConstants.CodeRemoteContentTypeName)] // required for ILanguageClient support
        [BaseDefinition(BicepLanguageServerClientConstants.JsonContentTypeName)]
        public ContentTypeDefinition? IBicepConfigContentType { get; set; }
    }
}
