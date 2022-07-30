// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.Utilities;

namespace Bicep.VSLanguageServerClient.ContentType
{
    /// <summary>
    /// VS lsp sends out events to languages based on content type and file extension.
    /// Content types are listed in <see cref="BicepLanguageServerClient"/>. Since we want to listen to changes in 
    /// bicepconfig.json files, we will create a separate content type and list it in <see cref="BicepLanguageServerClient"/>.
    /// We specifically set the fileName here to make sure events get sent across only for bicepconfig.json changes.
    /// </summary>
    [Export(typeof(IFilePathToContentTypeProvider))]
    [Name(nameof(BicepConfigContentTypeProvider))]
    [FileName(BicepLanguageServerClientConstants.BicepConfigFileName)]
    public class BicepConfigContentTypeProvider : IFilePathToContentTypeProvider
    {
        private readonly IContentTypeRegistryService contentTypeRegistryService;

        [ImportingConstructor]
        public BicepConfigContentTypeProvider(IContentTypeRegistryService contentTypeRegistryService)
        {
            this.contentTypeRegistryService = contentTypeRegistryService;
        }

        public bool TryGetContentTypeForFilePath(string filePath, out IContentType? contentType)
        {
            var fileName = Path.GetFileName(filePath);

            if (fileName.Equals(BicepLanguageServerClientConstants.BicepConfigFileName, StringComparison.OrdinalIgnoreCase))
            {
                contentType = contentTypeRegistryService.GetContentType(BicepLanguageServerClientConstants.BicepConfigContentType);
                return true;
            }

            contentType = null;
            return false;
        }
    }
}
