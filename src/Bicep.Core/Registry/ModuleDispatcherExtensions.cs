// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Registry
{
    public static class ModuleDispatcherExtensions
    {
        public static ResultWithDiagnosticBuilder<ArtifactReference> TryGetModuleReference(this IModuleDispatcher moduleDispatcher, BicepSourceFile referencingFile, string reference)
            => moduleDispatcher.TryGetArtifactReference(referencingFile, ArtifactType.Module, reference);
    }
}
