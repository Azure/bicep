// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.Utilities;

namespace Bicep.VSLanguageServerClient.ContentType
{
    [Export(typeof(IFilePathToContentTypeProvider))]
    [Name("BicepConfigContentTypeProvider")]
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

            if (fileName.Equals("bicepconfig.json", StringComparison.Ordinal))
            {
                contentType = contentTypeRegistryService.GetContentType(BicepConfigContentTypeDefinition.ContentType);
                return true;
            }

            contentType = null;
            return false;
        }
    }
}
