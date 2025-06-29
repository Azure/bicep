// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace Bicep.VSLanguageServerClient.ContentType
{
    public class BicepParamContentTypeDefinitionImpl
    {
        /// <summary>
        /// Exports the bicep content type.
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(BicepLanguageServerClientConstants.BicepParamContentType)]
        [BaseDefinition(BicepLanguageServerClientConstants.CodeRemoteContentTypeName)] // required for ILanguageClient support
        public ContentTypeDefinition? IBicepParamContentType { get; set; }


        /// <summary>
        /// Exports the bicep file extension
        /// </summary>
        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(BicepLanguageServerClientConstants.BicepParamContentType)]
        [FileExtension(BicepLanguageServerClientConstants.BicepParamFileExtension)]
        public FileExtensionToContentTypeDefinition? IBicepParamFileExtension { get; set; }
    }
}
