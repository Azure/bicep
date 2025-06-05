// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.IO.Abstraction;

namespace Bicep.Core.ArtifactCache
{
    public class TemplateSpecModuleCacheAccessor
    {
        private readonly IDirectoryHandle cacheDirectory;

        public TemplateSpecModuleCacheAccessor(string subscriptionId, string resourceGroupName, string templateSpecName, string version, IDirectoryHandle rootCacheDirectory)
        {
            this.cacheDirectory = rootCacheDirectory.GetDirectory($"{ArtifactReferenceSchemes.TemplateSpecs}/{subscriptionId}/{resourceGroupName}/{templateSpecName}/{version}".ToLowerInvariant());
        }

        public IFileHandle MainTemplateSpecFile => this.cacheDirectory.GetFile("main.json");
    }
}
