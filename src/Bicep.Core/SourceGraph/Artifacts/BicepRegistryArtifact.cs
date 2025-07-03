// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Registry.Oci;
using Bicep.IO.Abstraction;

namespace Bicep.Core.SourceGraph.Artifacts
{
    public abstract class BicepRegistryArtifact : ExternalArtifact
    {

        public BicepRegistryArtifact(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
            : base(ResolveCacheDirectory(address, rootCacheDirectory))
        {
        }

        public IFileHandle LockFile => this.GetFile("lock");

        public IFileHandle ManifestFile => this.GetFile("manifest");

        public IFileHandle MetadataFile => this.GetFile("metadata");

        private static IDirectoryHandle ResolveCacheDirectory(IOciArtifactAddressComponents address, IDirectoryHandle rootCacheDirectory)
        {
            // rootCacheDirectory is already set to %userprofile%\.bicep\br or ~/.bicep/br by default depending on OS
            // we need to split each component of the reference into a sub directory to fit within the max file name length limit on linux and mac

            // TODO: Need to deal with IDNs (internationalized domain names)
            // domain names can only contain alphanumeric chars, _, -, and numbers (example.azurecr.io or example.azurecr.io:443)
            // IPV4 addresses only contain dots and numbers (127.0.0.1 or 127.0.0.1:5000)
            // IPV6 addresses are hex digits with colons (2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF or [2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF]:5000)
            // the only problematic character is the colon, which we will replace with $ which is not allowed in any of the possible registry values
            // we will also normalize casing for registries since they are case-insensitive
            var registry = address.Registry.Replace(':', '$').ToLowerInvariant();

            // modules can have mixed hierarchy depths so we will flatten a repository into a single directory name
            // however to do this we must get rid of slashes (not a valid file system char on windows and a directory separator on linux/mac)
            // the replacement char must one that is not valid in a repository string but is valid in common type systems
            var repository = address.Repository.Replace('/', '$');

            var tagOrDigest = (address.Tag, address.Digest) switch
            {
                (not null, null) => TagEncoder.Encode(address.Tag),
                (null, not null) => address.Digest.Replace(':', '#'),
                _ => throw new InvalidOperationException("Either tag or digest must be set."),
            };

            return rootCacheDirectory.GetDirectory($"{OciArtifactReferenceFacts.Scheme}/{registry}/{repository}/{tagOrDigest}");
        }

    }
}
