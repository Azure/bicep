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
        [Name(BicepConfigContentTypeDefinition.ContentType)]
        [BaseDefinition(ExternalContentTypeDefinition.CodeRemoteContentTypeName)] // required for ILanguageClient support
        [FileName("bicepconfig.json")]
        public ContentTypeDefinition? IBicepConfigContentType { get; set; }
    }
}
