// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.References;
using Bicep.Core.Utils;

namespace Bicep.Core.SourceLink
{
    public static class OciModuleReferenceExtensions
    {
        public static ResultWithException<SourceArchive> TryLoadSourceArchive(this OciArtifactReference moduleReference) =>
            SourceArchive.TryUnpackFromFile(moduleReference.ModuleCacheEntry.SourceTgzFile);
    }
}
