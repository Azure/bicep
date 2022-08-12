// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Bicep.VSLanguageServerClient.ContentType
{
    public class BicepContentTypeDefinitionImpl
    {
        /// <summary>
        /// Exports the bicep content type.
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(BicepLanguageServerClientConstants.BicepContentType)]
        [BaseDefinition(BicepLanguageServerClientConstants.CodeRemoteContentTypeName)] // required for ILanguageClient support
        public ContentTypeDefinition? IBicepContentType { get; set; }

        /// <summary>
        /// Exports the bicep file extension
        /// </summary>
        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(BicepLanguageServerClientConstants.BicepContentType)]
        [FileExtension(BicepLanguageServerClientConstants.BicepFileExtension)]
        public FileExtensionToContentTypeDefinition? IBicepFileExtension { get; set; }
    }
}
