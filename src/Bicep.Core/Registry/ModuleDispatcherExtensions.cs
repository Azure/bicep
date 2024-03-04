// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;

namespace Bicep.Core.Registry
{
    public static class ModuleDispatcherExtensions
    {
        public static ResultWithDiagnostic<ArtifactReference> TryGetModuleReference(this IModuleDispatcher moduleDispatcher, string reference, Uri parentModuleUri)
            => moduleDispatcher.TryGetArtifactReference(ArtifactType.Module, reference, parentModuleUri);
    }
}
