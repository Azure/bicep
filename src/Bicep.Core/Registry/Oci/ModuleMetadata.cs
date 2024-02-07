// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Registry.Oci
{
    public class ModuleMetadata(string manifestDigest)
    {
        public string ManifestDigest { get; } = manifestDigest;
    }
}
