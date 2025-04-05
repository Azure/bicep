// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;
using Bicep.IO.Utils;

namespace Bicep.Core.ArtifactCache
{
    public sealed class OciModuleCacheAccessor : OciArtifactCacheAccessor
    {
        public OciModuleCacheAccessor(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(address, rootCacheDirectory)
        {
        }

        public IFileHandle EntryPointFile => this.GetFile("main.json");

        public TgzFileHandle SourceTgzFile => new(this.GetFile("source.tgz"));
    }
}
