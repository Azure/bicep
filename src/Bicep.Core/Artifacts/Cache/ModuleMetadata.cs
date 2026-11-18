// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Bicep.Core.Artifacts.Cache
{
    public class ModuleMetadata
    {
        public ModuleMetadata(string manifestDigest)
        {
            this.ManifestDigest = manifestDigest;
        }

        public string ManifestDigest { get; }
    }
}
