// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph.Artifacts
{
    public class TemplateSpecModuleArtifact
    {
        private readonly IDirectoryHandle cacheDirectory;

        public TemplateSpecModuleArtifact(string subscriptionId, string resourceGroupName, string templateSpecName, string version, IDirectoryHandle rootCacheDirectory)
        {
            this.cacheDirectory = rootCacheDirectory.GetDirectory($"{ArtifactReferenceSchemes.TemplateSpecs}/{subscriptionId}/{resourceGroupName}/{templateSpecName}/{version}".ToLowerInvariant());
        }

        public IFileHandle MainTemplateSpecFile => this.cacheDirectory.GetFile("main.json");
    }
}
