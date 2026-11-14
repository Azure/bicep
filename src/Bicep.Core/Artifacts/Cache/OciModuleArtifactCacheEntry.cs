// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Extensions;
using Bicep.Core.RegistryClients.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Cache
{
    public sealed class OciModuleArtifactCacheEntry : OciArtifactCacheEntry
    {
        public OciModuleArtifactCacheEntry(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(address, rootCacheDirectory)
        {
        }

        public IFileHandle MainTemplateFile => this.GetFile("main.json");

        public IFileHandle SourceTgzFile => this.GetFile("source.tgz");

        public override IFileHandle EntryPoint => this.MainTemplateFile;

        /// <summary>
        /// Writes a pulled module into the cache, unless another compilation already has.
        /// </summary>
        public Task WriteAsync(ModuleArtifactPackage package, OciArtifactPayload payload) => this.WriteAsync(payload, () =>
        {
            this.MainTemplateFile.Write(package.Template);

            if (package.Sources is { } sources)
            {
                this.SourceTgzFile.Write(sources);
            }
        });
    }
}