// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Cache
{
    /// <summary>
    /// The cache entry of a Template Spec: <c>ts/&lt;subscription&gt;/&lt;resource group&gt;/&lt;name&gt;/&lt;version&gt;/</c> under
    /// the cache root, in lower case.
    /// </summary>
    public class TemplateSpecArtifactCacheEntry : ArtifactCacheEntry
    {
        public TemplateSpecArtifactCacheEntry(string subscriptionId, string resourceGroupName, string templateSpecName, string version, IDirectoryHandle rootCacheDirectory)
            : base(rootCacheDirectory.GetDirectory($"{ArtifactSchemes.TemplateSpecs}/{subscriptionId}/{resourceGroupName}/{templateSpecName}/{version}".ToLowerInvariant()))
        {
        }

        public IFileHandle MainTemplateSpecFile => this.GetFile("main.json");

        public override IFileHandle EntryPoint => this.MainTemplateSpecFile;

        public override bool IsComplete => this.MainTemplateSpecFile.Exists();

        /// <summary>
        /// Writes a downloaded Template Spec into the cache, unless another compilation already has.
        /// </summary>
        public Task WriteAsync(string templateSpecContent) => this.WriteAsync(() => this.MainTemplateSpecFile.Write(templateSpecContent));
    }
}